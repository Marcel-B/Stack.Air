using com.b_velop.stack.Air.Models;
using System.Text.Json;
using NUnit.Framework;
using System.Linq;

namespace Air.Tests
{
    public class DeserializerTests
    {
        private readonly string _airData =
            @"{
                ""esp8266id"": ""2063272"",
                ""software_version"": ""NRZ-2019-125-B1"",
                ""sensordatavalues"": [
                        {
                            ""value_type"": ""SDS_P1"",
                            ""value"": ""51.63""
                        },
                        {
                            ""value_type"": ""SDS_P2"",
                            ""value"": ""24.13""
                        }
                    ]
                }";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void DeserializerTests_Deserialize_IsInstanceOf()
        {
            var tc = JsonSerializer.Deserialize<AirDto>(_airData);

            Assert.IsInstanceOf<AirDto>(tc);
        }

        [Test]
        public void DeserializerTests_Deserialize_IsNotNull()
        {
            var tc = JsonSerializer.Deserialize<AirDto>(_airData);

            Assert.IsNotNull(tc);
        }

        [TestCase("SDS_P1", "51.63", 0)]
        [TestCase("SDS_P2", "24.13", 1)]
        public void DeserializerTests_Deserialize_PropertiesNotNull(
            string valueType,
            string value,
            int idx)
        {
            var tc = JsonSerializer.Deserialize<AirDto>(_airData);

            Assert.AreEqual(valueType, tc.SensorDataValues.ToList()[idx].ValueType);
            Assert.AreEqual(value, tc.SensorDataValues.ToList()[idx].Value);
        }
    }
}