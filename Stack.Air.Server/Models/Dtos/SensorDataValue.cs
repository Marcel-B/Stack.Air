using System.Text.Json.Serialization;

namespace com.b_velop.Stack.Air.Server.Models.Dtos
{
    public class SensorDataValue
        {
            [JsonPropertyName("value_type")]
            public string ValueType { get; set; }

            [JsonPropertyName("value")]
            public string Value { get; set; }
        }
    }
