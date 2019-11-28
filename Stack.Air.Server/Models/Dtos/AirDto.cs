using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.b_velop.Stack.Air.Server.Models.Dtos
{
    public class AirDto
        {
            [JsonPropertyName("esp8266id")]
            public string Esp8266Id { get; set; }

            [JsonPropertyName("software_version")]
            public string SoftwareVersion { get; set; }

            [JsonPropertyName("sensordatavalues")]
            public IEnumerable<SensorDataValue> SensorDataValues { get; set; }
        }
    }
