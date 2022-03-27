using MetarDecoder.ChunkDecoder.Abstract;
using MetarDecoder.Entity;
using MetarDecoder.Exceptions;
using System.Collections.Generic;

namespace MetarDecoder.ChunkDecoder
{
    public sealed class WindShearChunkDecoder : MetarChunkDecoder
    {
        public const string WindshearAllRunwaysParameterName = "WindshearAllRunways";
        public const string WindshearRunwaysParameterName = "WindshearRunways";

        private const string RunwayRegexPattern = "WS R(WY)?([0-9]{2}[LCR]?)";

        public override string GetRegex()
        {
            return $@"^(WS ALL RWY|({RunwayRegexPattern})( {RunwayRegexPattern})?( {RunwayRegexPattern})?)( )";
        }

        public override Dictionary<string, object> Parse(string remainingMetar, bool withCavok = false)
        {
            var consumed = Consume(remainingMetar);
            var found = consumed.Value;
            var newRemainingMetar = consumed.Key;
            var result = new Dictionary<string, object>();

            bool? all = null;
            var runways = new List<string>();

            if (found.Count > 1)
            {
                // detect if we have windshear on all runway or only one
                if (found[1].Value == "WS ALL RWY")
                {
                    all = true;
                    runways = null;
                }
                else
                {
                    // one or more runways, build array
                    all = false;

                    for (var k = 2; k < 9; k += 3)
                    {
                        if (!string.IsNullOrEmpty(found[k].Value))
                        {
                            var runway = found[k + 2].Value;
                            var qfuAsInt = Value.ToInt(runway);
                            // check runway qfu validity
                            if (qfuAsInt > 36 || qfuAsInt < 1)
                            {
                                throw new MetarChunkDecoderException(remainingMetar, newRemainingMetar, MetarChunkDecoderException.Messages.InvalidRunwayQFURunwaVisualRangeInformation, this);
                            }
                            runways.Add(runway);
                        }
                    }
                }
            }

            result.Add(WindshearAllRunwaysParameterName, all);
            result.Add(WindshearRunwaysParameterName, runways);

            return GetResults(newRemainingMetar, result);
        }
    }
}
