using MetarDecoder.ChunkDecoder;
using MetarDecoder.ChunkDecoder.Abstract;
using MetarDecoder.Entity;
using MetarDecoder.Exceptions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace MetarDecoder
{
    public sealed class Decoder
    {
        public const string ResultKey = "Result";
        public const string RemainingMetarKey = "RemainingMetar";
        public const string ExceptionKey = "Exception";

        private static ReadOnlyCollection<MetarChunkDecoder> _decoderChain = new ReadOnlyCollection<MetarChunkDecoder>(new List<MetarChunkDecoder>()
        {
            new ReportTypeChunkDecoder(),
            new IcaoChunkDecoder(),
            new DatetimeChunkDecoder(),
            new ReportStatusChunkDecoder(),
            new SurfaceWindChunkDecoder(),
            new VisibilityChunkDecoder(),
            new RunwayVisualRangeChunkDecoder(),
            new PresentWeatherChunkDecoder(),
            new CloudChunkDecoder(),
            new TemperatureChunkDecoder(),
            new PressureChunkDecoder(),
            new RecentWeatherChunkDecoder(),
            new WindShearChunkDecoder(),
        });

        private bool _globalStrictParsing = false;

        /// <summary>
        /// Set global parsing mode (strict/not strict) for the whole object.
        /// </summary>
        /// <param name="isStrict"></param>
        public void SetStrictParsing(bool isStrict)
        {
            _globalStrictParsing = isStrict;
        }

        /// <summary>
        /// Decode a full metar string into a complete metar object
        /// while using global strict option.
        /// </summary>
        /// <param name=""></param>
        public DecodedMetar Parse(string rawMetar)
        {
            return ParseWithMode(rawMetar, _globalStrictParsing);
        }

        /// <summary>
        /// Decode a full metar string into a complete metar object
        /// with strict option, meaning decoding will stop as soon as
        /// a non-compliance is detected.
        /// </summary>
        /// <param name="rawMetar"></param>
        /// <returns></returns>
        public DecodedMetar ParseStrict(string rawMetar)
        {
            return ParseWithMode(rawMetar, true);
        }

        /// <summary>
        /// Decode a full metar string into a complete metar object
        /// ith strict option disabled, meaning that decoding will
        /// continue even if metar is not compliant.
        /// </summary>
        /// <param name="rawMetar"></param>
        /// <returns></returns>
        public DecodedMetar ParseNotStrict(string rawMetar)
        {
            return ParseWithMode(rawMetar, false);
        }


        /// <summary>
        /// Decode a full metar string into a complete metar object.
        /// </summary>
        /// <param name="rawMetar"></param>
        /// <returns></returns>
        public static DecodedMetar ParseWithMode(string rawMetar, bool isStrict = false)
        {
            // prepare decoding inputs/outputs: (upper case, trim,
            // remove 'end of message', no more than one space)
            var cleanMetar = rawMetar.ToUpper().Trim();
            cleanMetar = Regex.Replace(cleanMetar, "=$", string.Empty);
            cleanMetar = Regex.Replace(cleanMetar, "[ ]{2,}", " ") + " ";
            var remainingMetar = cleanMetar;
            var decodedMetar = new DecodedMetar(cleanMetar);
            var withCavok = false;

            // call each decoder in the chain and use results to populate decoded metar
            foreach (var chunkDecoder in _decoderChain)
            {
                try
                {
                    // try to parse a chunk with current chunk decoder
                    var decodedData = TryParsing(chunkDecoder, isStrict, remainingMetar, withCavok);

                    // log any exception that would have occur at primary decoding
                    if (decodedData.ContainsKey(ExceptionKey))
                    {
                        decodedMetar.AddDecodingException((MetarChunkDecoderException)decodedData[ExceptionKey]);
                    }

                    // map obtained fields (if any) to the final decoded object
                    if (decodedData.ContainsKey(ResultKey) && decodedData[ResultKey] is Dictionary<string, object>)
                    {
                        var result = decodedData[ResultKey] as Dictionary<string, object>;
                        foreach (var obj in result)
                        {
                            if (obj.Value != null)
                            {
                                typeof(DecodedMetar).GetProperty(obj.Key).SetValue(decodedMetar, obj.Value);
                            }
                        }
                    }

                    // update remaining metar for next round
                    remainingMetar = decodedData[RemainingMetarKey] as string;
                }
                catch (MetarChunkDecoderException metarChunkDecoderException)
                {
                    // log error in decoded metar
                    decodedMetar.AddDecodingException(metarChunkDecoderException);
                    // abort decoding if strict mode is activated, continue otherwise
                    if (isStrict)
                    {
                        break;
                    }
                    // update remaining metar for next round
                    remainingMetar = metarChunkDecoderException.RemainingMetar;
                }

                // hook for report status decoder, abort if nil, but decoded metar is valid though
                if (chunkDecoder is ReportStatusChunkDecoder && decodedMetar.Status == "NIL")
                {
                    break;
                }

                // hook for CAVOK decoder, keep CAVOK information in memory
                if (chunkDecoder is VisibilityChunkDecoder)
                {
                    withCavok = decodedMetar.Cavok;
                }
            }

            return decodedMetar;
        }

        private static Dictionary<string, object> TryParsing(IMetarChunkDecoder chunkDecoder, bool strict, string remainingMetar, bool withCavok)
        {
            Dictionary<string, object> decoded;
            try
            {
                decoded = chunkDecoder.Parse(remainingMetar, withCavok);
            }
            catch (MetarChunkDecoderException primaryException)
            {
                if (strict)
                {
                    throw;
                }
                else
                {
                    try
                    {
                        //the PHP version of ConsumeOneChunk implements an additional, unused strict flag
                        var alternativeRemainingMetar = MetarChunkDecoder.ConsumeOneChunk(remainingMetar);
                        decoded = chunkDecoder.Parse(alternativeRemainingMetar, withCavok);
                        decoded.Add(ExceptionKey, primaryException);
                    }
                    catch (MetarChunkDecoderException)
                    {
                        throw primaryException;
                    }
                }
            }
            return decoded;
        }
    }
}
