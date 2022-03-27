using MetarDecoder.ChunkDecoder.Abstract;
using MetarDecoder.Entity;
using System.Collections.Generic;

namespace MetarDecoder.ChunkDecoder
{
    public sealed class TemperatureChunkDecoder : MetarChunkDecoder
    {
        public const string AirTemperatureParameterName = "AirTemperature";
        public const string DewPointTemperatureParameterName = "DewPointTemperature";

        private const string TempRegexPattern = "(M?[0-9]{2})";

        public override string GetRegex()
        {
            return $"^{TempRegexPattern}/{TempRegexPattern}?( )";
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
                if (!string.IsNullOrEmpty(found[1].Value))
                {
                    result.Add(AirTemperatureParameterName, new Value((double)Value.ToInt(found[1].Value), Value.Unit.DegreeCelsius));
                }
                if (!string.IsNullOrEmpty(found[2].Value))
                {
                    result.Add(DewPointTemperatureParameterName, new Value((double)Value.ToInt(found[2].Value), Value.Unit.DegreeCelsius));
                }
            }

            return GetResults(newRemainingMetar, result);
        }
    }
}
