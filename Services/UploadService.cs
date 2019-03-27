using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using com.b_velop.App.IdentityProvider;
using com.b_velop.App.IdentityProvider.Model;
using com.b_velop.stack.Air.BL;
using com.b_velop.stack.Classes.Dtos;
using GraphQL.Client;
using GraphQL.Common.Request;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
        private Token _token;
        private DateTime _exp;

        public UploadService(
            GraphQLRequest graphQlRequest,
            GraphQLClient graphQlClient,
            IOptions<ApiSecret> apiSecret,
            IIdentityProviderService service,
            ILogger<UploadService> logger)
        {
            _graphQlClient = graphQlClient;
            _graphQlRequest = graphQlRequest;
            _graphQlRequest.Query = @"mutation AddValue($measure: MeasureValueInput!) { createMeasureValue(measureValueType: $measure){id}}";
            _graphQlRequest.OperationName = "AddValue";
            _service = service;
            _apiSecret = apiSecret.Value;
            _logger = logger;
            _exp = DateTime.MinValue;
        }

        public async Task UpdateTokenAsync()
        {
            await SemaphoreSlim.WaitAsync();
            try
            {
                if (_token == null || _exp < DateTime.Now)
                {
                    var infoItem = new InfoItem(_apiSecret.ClientId, _apiSecret.ClientSecret, _apiSecret.Scope, _apiSecret.AuthorityUrl);
                    var url = _apiSecret.GraphQLUrl;
                    _token = await _service.GetTokenAsync(infoItem);
                    _exp = DateTime.Now.AddSeconds(_token.ExpiresIn);
                    if (_token == null)
                    {
                        _logger.LogError(2432, "Error occurred while fetch token.");
                        return;
                    }
                }
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        public async Task SendAsync(
            DateTimeOffset time,
            Guid id,
            double value)
        {
            _graphQlRequest.Variables = new { measure = new { Timestamp = time, Point = id, Value = value } };
            _graphQlClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token.AccessToken);
            var graphQlResponse = await _graphQlClient.PostAsync(_graphQlRequest);
            return;
        }

        public async Task ProcessDataAsync(
            AirdataDto airdata,
            DateTimeOffset timestamp)
        {
            try
            {
                foreach (var item in airdata.Sensordatavalues)
                {
                    switch (item.ValueType)
                    {
                        case "SDS_P1":
                            if (double.TryParse(item.Value, NumberStyles.Any, CultureInfo.CreateSpecificCulture("en-GB"), out var sdsP10))
                                await SendAsync(timestamp, BL.MeasurePoint.SDS011_PM10, sdsP10);
                            break;
                        case "SDS_P2":
                            var id = Guid.NewGuid();
                            if (double.TryParse(item.Value, NumberStyles.Any, CultureInfo.CreateSpecificCulture("en-GB"), out var sdsP2))
                                await SendAsync(timestamp, BL.MeasurePoint.SDS011_PM2_5, sdsP2);
                            break;
                        case "humidity":
                            if (double.TryParse(item.Value, NumberStyles.Any, CultureInfo.CreateSpecificCulture("en-GB"), out var humidity))
                                await SendAsync(timestamp, BL.MeasurePoint.DHT22_Humidity, humidity);
                            break;
                        case "temperature":
                            if (double.TryParse(item.Value, NumberStyles.Any, CultureInfo.CreateSpecificCulture("en-GB"), out var temperature))
                                await SendAsync(timestamp, BL.MeasurePoint.DHT22_Temperature, temperature);
                            break;
                        case "BMP_pressure":
                            if (double.TryParse(item.Value, NumberStyles.Any, CultureInfo.CreateSpecificCulture("en-GB"), out var pressure))
                                await SendAsync(timestamp, BL.MeasurePoint.BMP180_Luftdruck, (pressure / 100.0));
                            break;
                        case "BMP_temperature":
                            if (double.TryParse(item.Value, NumberStyles.Any, CultureInfo.CreateSpecificCulture("en-GB"), out var tempreatureBmp))
                                await SendAsync(timestamp, BL.MeasurePoint.BMP180_Temperature, tempreatureBmp);
                            break;
                        case "max_micro":
                            if (double.TryParse(item.Value, NumberStyles.Any, CultureInfo.CreateSpecificCulture("en-GB"), out var microMax))
                                await SendAsync(timestamp, BL.MeasurePoint.WiFi_MAXMICRO, microMax);
                            break;
                        case "min_micro":
                            if (double.TryParse(item.Value, NumberStyles.Any, CultureInfo.CreateSpecificCulture("en-GB"), out var microMin))
                                await SendAsync(timestamp, BL.MeasurePoint.WiFi_MINMICRO, microMin);
                            break;
                        case "samples":
                            if (double.TryParse(item.Value, NumberStyles.Any, CultureInfo.CreateSpecificCulture("en-GB"), out var samples))
                                await SendAsync(timestamp, BL.MeasurePoint.SAMPLES, samples);
                            break;
                        case "signal":
                            if (double.TryParse(item.Value, NumberStyles.Any, CultureInfo.CreateSpecificCulture("en-GB"), out var signal))
                                await SendAsync(timestamp, BL.MeasurePoint.WiFi_SIGNAL, signal);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(2422, ex, $"Error while inserting Luftdaten", airdata);
                return;
            }
        }

        public async Task<bool> UploadAsync(
            AirdataDto airdata,
            DateTimeOffset timestamp)
        {
            await UpdateTokenAsync();
            await ProcessDataAsync(airdata, timestamp);
            return true;
        }
    }
}