using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.b_velop.Stack.Air.Server.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace Stack.Air.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class AirController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<AirController> _logger;

        public AirController(ILogger<AirController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(
            AirDto value)
        {
            using (Metrics.CreateHistogram("stack_air_POST_air_duration_seconds", "").NewTimer())
            {
                var uploadValues = new List<double>();
                var uploadPoints = new List<Guid>();
                var values = new Dictionary<string, double>();
                try
                {
                    foreach (var item in value.SensorDataValues)
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
                        return Ok();

                    _cache.Set(Memory.Values, values);
                    _graphQlRequest.Variables = new { points = uploadPoints, values = uploadValues };
                    var result = await _graphQlClient.PostAsync(_graphQlRequest);
                    _logger.LogInformation($"Uploaded '{uploadValues.Count}' air values");
                }
                catch (Exception ex)
                {
                    _logger.LogError(2422, ex, $"Error while inserting '{uploadPoints.Count}' Luftdaten", airdata);
                    return new StatusCodeResult(500);
                }
                return Ok();
            }
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
