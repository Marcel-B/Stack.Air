using System;
using System.Collections;
using com.b_velop.stack.Classes.Models;
using com.b_velop.stack.Classes.Dtos;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace com.b_velop.stack.Air.BL
{
    public class AirMapper
    {
        private readonly ILogger<AirMapper> _logger;

        public AirMapper(
            ILogger<AirMapper> logger)
        {
            _logger = logger;
        }

        public IEnumerable<MeasureValue> MapAirValues(
            AirdataDto dto)
        {
            var time = DateTimeOffset.Now;
            var measureValues = new List<MeasureValue>();
            foreach (var item in dto.Sensordatavalues)
            {
                switch (item.ValueType)
                {
                    case "SDS_P1":
                        if (double.TryParse(item.Value, NumberStyles.Any, CultureInfo.CreateSpecificCulture("en-GB"), out var sdsP10))
                            measureValues.Add(new MeasureValue
                            {
                                Id = Guid.NewGuid(),
                                Value = sdsP10,
                                Point = BL.MeasurePoint.SDS011_PM10,
                                Timestamp = time
                            });
                        break;
                    case "SDS_P2":
                        var id = Guid.NewGuid();
                        if (double.TryParse(item.Value, NumberStyles.Any, CultureInfo.CreateSpecificCulture("en-GB"), out var sdsP2))
                            measureValues.Add(new MeasureValue
                            {
                                Id = Guid.NewGuid(),
                                Value = sdsP2,
                                Point = BL.MeasurePoint.SDS011_PM2_5,
                                Timestamp = time
                            });
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
            return measureValues;
        }
    }
}
