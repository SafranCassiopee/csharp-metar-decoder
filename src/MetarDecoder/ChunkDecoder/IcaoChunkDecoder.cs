using MetarDecoder.ChunkDecoder.Abstract;
using MetarDecoder.Exceptions;
using System.Collections.Generic;

namespace MetarDecoder.ChunkDecoder
{
    public sealed class IcaoChunkDecoder : MetarChunkDecoder
    {
        public const string ICAOParameterName = "ICAO";

        public override string GetRegex()
        {
            return "^([A-Z0-9]{4}) ";
        }

        public override Dictionary<string, object> Parse(string remainingMetar, bool withCavok = false)
        {
            var consumed = Consume(remainingMetar);
            var found = consumed.Value;
            var newRemainingMetar = consumed.Key;
            var result = new Dictionary<string, object>();

            // handle the case where nothing has been found
            if (found.Count <= 1)
            {
                throw new MetarChunkDecoderException(remainingMetar, newRemainingMetar, MetarChunkDecoderException.Messages.ICAONotFound, this);
            }

            // retrieve found params
            result.Add(ICAOParameterName, found[1].Value);

            return GetResults(newRemainingMetar, result);
        }
    }
}
