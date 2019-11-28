using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using com.b_velop.stack.DataContext.Entities;
using com.b_velop.stack.DataContext.Repository;
using com.b_velop.Stack.Air.Server.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace Stack.Air.Server.Controllers
{
    public static class SensorTypes
    {
        public const string SdsP1 = "SDS_P1";
        public const string SdsP2 = "SDS_P2";
        public const string Humidity = "humidity";
        public const string Temperature = "temperature";
        public const string BmpPressure = "BMP_pressure";
        public const string BmpTemperature = "BMP_temperature";
    }

    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class AirController : ControllerBase
    {
        private readonly IRepositoryWrapper _rep;
        private readonly ILogger<AirController> _logger;

        public AirController(
            IRepositoryWrapper rep,
            ILogger<AirController> logger)
        {
            _rep = rep;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(
            AirDto airData)
        {
            using (Metrics.CreateHistogram("stack_air_POST_air_duration_seconds", "").NewTimer())
            {
                var measureValues = new List<MeasureValue>();
                try
                {
                    foreach (var sensorData in airData.SensorDataValues)
                    {
                        var sensorId = sensorData.ValueType switch
                        {
                            SensorTypes.SdsP1 => new Guid("777CECC4-C140-477D-BD94-5A0A611F47FC"),
                            SensorTypes.SdsP2 => new Guid("FB43A587-8251-4EA1-97B2-6F2F702952A6"),
                            SensorTypes.Humidity => new Guid("795F28B0-77ED-4A57-AF57-32A2C47CDBA0"),
                            SensorTypes.Temperature => new Guid("6E78294C-0AB6-4E71-A790-EA099D0693A6"),
                            SensorTypes.BmpPressure => new Guid("516C6AB3-E615-462E-8718-63FD85220D6A"),
                            SensorTypes.BmpTemperature => new Guid("8FA026A5-BA9F-476A-AB7F-27406C3CEA91"),
                            _ => Guid.Empty
                        };

                        if (sensorId == Guid.Empty)
                            continue;

                        if (!double.TryParse(sensorData.Value, NumberStyles.Any, CultureInfo.CreateSpecificCulture("en-GB"), out var value))
                            continue;

                        measureValues.Add(new MeasureValue
                        {
                            Created = DateTimeOffset.Now,
                            Id = Guid.NewGuid(),
                            Point = sensorId,
                            Timestamp = DateTimeOffset.Now,
                            Value = value
                        });
                    }

                    if (measureValues.Count == 0)
                        return Ok();

                    _ = await _rep.MeasureValue.InsertBunchAsync(measureValues);
                }
                catch (Exception ex)
                {
                    _logger.LogError(2422, ex, $"Error while inserting '{measureValues.Count}' Luftdaten", airData);
                    return new StatusCodeResult(500);
                }
                return Ok();
            }
        }
    }
}
