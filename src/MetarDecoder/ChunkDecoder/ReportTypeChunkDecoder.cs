using MetarDecoder.ChunkDecoder.Abstract;
using System.Collections.Generic;
using static MetarDecoder.Entity.DecodedMetar;

namespace MetarDecoder.ChunkDecoder
{
    public sealed class ReportTypeChunkDecoder : MetarChunkDecoder
    {
        public const string TypeParameterName = "Type";

        public override string GetRegex()
        {
            return "((METAR|SPECI)( COR){0,1}) ";
        }

        public override Dictionary<string, object> Parse(string remainingMetar, bool withCavok = false)
        {
            var consumed = Consume(remainingMetar);
            var found = consumed.Value;
            var newRemainingMetar = consumed.Key;
            var result = new Dictionary<string, object>();

            if (found.Count > 1)
            {
                var type = MetarType.NULL;
                switch (found[1].Value)
                {
                    case "METAR":
                        type = MetarType.METAR;
                        break;
                    case "SPECI":
                        type = MetarType.SPECI;
                        break;
                    case "METAR COR":
                        type = MetarType.METAR_COR;
                        break;
                    case "SPECI COR":
                        type = MetarType.SPECI_COR;
                        break;
                }

                result.Add(TypeParameterName, type);
            }
            else
            {
                result.Add(TypeParameterName, MetarType.NULL);
            }

            return GetResults(newRemainingMetar, result);
        }
    }
}
