using MetarDecoder.ChunkDecoder.Abstract;
using MetarDecoder.Entity;
using MetarDecoder.Exceptions;
using System.Collections.Generic;
using static MetarDecoder.Entity.Value;

namespace MetarDecoder.ChunkDecoder
{
    /// <summary>
    /// Chunk decoder for atmospheric pressure section.
    /// </summary>
    public sealed class PressureChunkDecoder : MetarChunkDecoder
    {
        public const string PressureParameterName = "Pressure";

        public override string GetRegex()
        {
            return "^(Q|A)(////|[0-9]{4})( )";
        }

        public override Dictionary<string, object> Parse(string remainingMetar, bool withCavok = false)
        {
            var consumed = Consume(remainingMetar);
            var found = consumed.Value;
            var newRemainingMetar = consumed.Key;
            var result = new Dictionary<string, object>();

            if (found.Count <= 1)
            {
                throw new MetarChunkDecoderException(remainingMetar, newRemainingMetar, MetarChunkDecoderException.Messages.AtmosphericPressureNotFound, this);
            }

            double raw_value;
            Value value = null;
            if (found[2].Value != "////")
            {
                raw_value = ToInt(found[2].Value).Value;
                // convert value if needed
                if (found[1].Value == "A")
                {
                    raw_value = raw_value / 100;
                }
                var units = Unit.None;
                switch (found[1].Value)
                {
                    case "Q":
                        units = Unit.HectoPascal;
                        break;
                    case "A":
                        units = Unit.MercuryInch;
                        break;
                }
                value = new Value(raw_value, units);
            }
            result.Add(PressureParameterName, value);

            return GetResults(newRemainingMetar, result);
        }
    }
}
