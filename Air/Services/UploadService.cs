﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using com.b_velop.IdentityProvider;
using com.b_velop.IdentityProvider.Model;
using com.b_velop.stack.Air.BL;
using com.b_velop.stack.Air.Constants;
using com.b_velop.stack.Air.Models;
using GraphQL.Client;
using GraphQL.Common.Request;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace com.b_velop.stack.Air.Services
{

    public class UploadService : IUploadService
    {
        private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly GraphQLClient _graphQlClient;
        private readonly GraphQLRequest _graphQlRequest;
        private readonly IIdentityProviderService _service;
        private readonly ApiSecret _apiSecret;
        private readonly ILogger<UploadService> _logger;
        private DateTime _exp;
        private IMemoryCache _cache;
        private IDictionary<string, Guid> _map;

        public UploadService(
            GraphQLRequest graphQlRequest,
            GraphQLClient graphQlClient,
            ApiSecret apiSecret,
            IIdentityProviderService service,
            IMemoryCache cache,
            ILogger<UploadService> logger)
        {
            _map = new Dictionary<string, Guid>
            {
                { "SDS_P1", new Guid("777CECC4-C140-477D-BD94-5A0A611F47FC")},
                { "SDS_P2", new Guid("FB43A587-8251-4EA1-97B2-6F2F702952A6")},
                { "humidity", new Guid("795F28B0-77ED-4A57-AF57-32A2C47CDBA0")},
                { "temperature", new Guid("6E78294C-0AB6-4E71-A790-EA099D0693A6")},
                { "BMP_pressure", new Guid("516C6AB3-E615-462E-8718-63FD85220D6A")},
                { "BMP_temperature", new Guid("8FA026A5-BA9F-476A-AB7F-27406C3CEA91")},
            };
            _graphQlClient = graphQlClient;
            _graphQlRequest = graphQlRequest;
            _graphQlRequest.Query = Query.CreateMeasureValueBunch;
            //@"mutation AddValue($measure: MeasureValueInput!) { createMeasureValue(measureValueType: $measure){id}}";
            //_graphQlRequest.OperationName = "AddValue";
            _service = service;
            _apiSecret = apiSecret;
            _cache = cache;
            _logger = logger;
            _exp = DateTime.MinValue;
        }

        public async Task<Token> UpdateTokenAsync()
        {
            using (Metrics.CreateHistogram("stack_air_update_token_async_duration_seconds", "").NewTimer())
            {
                await SemaphoreSlim.WaitAsync();
                try
                {
                    var infoItem = new InfoItem(_apiSecret.ClientId, _apiSecret.ClientSecret, _apiSecret.Scope, _apiSecret.AuthorityUrl);
                    var url = _apiSecret.GraphQLUrl;
                    var token = await _service.GetTokenAsync(infoItem);
                    if (token == null)
                    {
                        _logger.LogError(2432, "Error occurred while fetch token.");
                        return null;
                    }
                    return token;
                }
                finally
                {
                    SemaphoreSlim.Release();
                }
            }
        }

        public async Task ProcessDataAsync(
            AirDto airdata)
        {
            var uploadValues = new List<double>();
            var uploadPoints = new List<Guid>();
            var values = new Dictionary<string, double>();
            try
            {
                foreach (var item in airdata.SensorDataValues)
                {
                    if (!_map.ContainsKey(item.ValueType))
                        continue;
                    if (!double.TryParse(item.Value, NumberStyles.Any, CultureInfo.CreateSpecificCulture("en-GB"), out var value))
                        continue;
                    uploadValues.Add(value);
                    uploadPoints.Add(_map[item.ValueType]);
                    values[item.ValueType] = value;
                }

                if (uploadValues.Count == 0)
                    return;

                _cache.Set(Memory.Values, values);
                _graphQlRequest.Variables = new { points = uploadPoints, values = uploadValues };
                var result = await _graphQlClient.PostAsync(_graphQlRequest);
                _logger.LogInformation($"Uploaded '{uploadValues.Count}' air values");
            }
            catch (Exception ex)
            {
                _logger.LogError(2422, ex, $"Error while inserting '{uploadPoints.Count}' Luftdaten", airdata);
                return;
            }
        }

        public async Task<bool> UploadAsync(
            AirDto airdata,
            DateTimeOffset timestamp)
        {
            if (!_cache.TryGetValue(Memory.Token, out Token token))
            {
                token = await UpdateTokenAsync();
                _cache.Set(Memory.Token, token);
                _cache.Set(Memory.Time, DateTime.Now.AddSeconds(token.ExpiresIn));
            }
            if (!_cache.TryGetValue(Memory.Time, out DateTime time) || time <= DateTime.Now)
            {
                token = await UpdateTokenAsync();
                _cache.Set(Memory.Token, token);
                _cache.Set(Memory.Time, DateTime.Now.AddSeconds(token.ExpiresIn));
            }

            _graphQlClient.DefaultRequestHeaders.Clear();
            _graphQlClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);

            await ProcessDataAsync(airdata);
            return true;
        }
    }
}