using MetarDecoder.ChunkDecoder.Abstract;
using MetarDecoder.Exceptions;
using System.Collections.Generic;

namespace MetarDecoder.ChunkDecoder
{
    public sealed class ReportStatusChunkDecoder : MetarChunkDecoder
    {
        public const string StatusParameterName = "Status";

        public override string GetRegex()
        {
            return "^([A-Z]+) ";
        }

        public override Dictionary<string, object> Parse(string remainingMetar, bool withCavok = false)
        {
            var consumed = Consume(remainingMetar);
            var found = consumed.Value;
            var newRemainingMetar = consumed.Key;
            var result = new Dictionary<string, object>();

            var status = string.Empty;

            if (found.Count > 1)
            {
                status = found[1].Value;
                if (status.Length != 3 && status != "AUTO")
                {
                    throw new MetarChunkDecoderException(remainingMetar, newRemainingMetar, MetarChunkDecoderException.Messages.InvalidReportStatus, this);
                }
                // retrieve found params
                result.Add(StatusParameterName, status);
            }
            else
            {
                result.Add(StatusParameterName, string.Empty);
            }

            if (result.Count > 0 && result[StatusParameterName] as string == "NIL" && newRemainingMetar.Trim().Length > 0)
            {
                throw new MetarChunkDecoderException(remainingMetar, newRemainingMetar, MetarChunkDecoderException.Messages.NoInformationExpectedAfterNILStatus, this);
            }

            return GetResults(newRemainingMetar, result);
        }
    }
}
