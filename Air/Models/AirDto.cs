using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace com.b_velop.stack.Air.Models
{
    public class SensorDataValue
    {
        [JsonPropertyName("value_type")]
        public string ValueType { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
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
