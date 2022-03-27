using MetarDecoder.ChunkDecoder.Abstract;
using MetarDecoder.Entity;
using System.Collections.Generic;

namespace MetarDecoder.ChunkDecoder
{
    public sealed class RecentWeatherChunkDecoder : MetarChunkDecoder
    {
        public const string RecentWeatherParameterName = "RecentWeather";

        public override string GetRegex()
        {
            return $"^RE({PresentWeatherChunkDecoder.CaracRegexPattern})?({PresentWeatherChunkDecoder.TypeRegexPattern})?({PresentWeatherChunkDecoder.TypeRegexPattern})?({PresentWeatherChunkDecoder.TypeRegexPattern})?()? ";
        }

        public override Dictionary<string, object> Parse(string remainingMetar, bool withCavok = false)
        {
            var consumed = Consume(remainingMetar);
            var found = consumed.Value;
            var newRemainingMetar = consumed.Key;
            var result = new Dictionary<string, object>();

            if (found.Count > 1)
            {
                // retrieve found params
                var weather = new WeatherPhenomenon();
                weather.Characteristics = found[1].Value;
                for (var k = 2; k <= 4; ++k)
                {
                    if (!string.IsNullOrEmpty(found[k].Value))
                    {
                        weather.AddType(found[k].Value);
                    }
                }
                result.Add(RecentWeatherParameterName, weather);
            }

            return GetResults(newRemainingMetar, result);
        }
    }
}
