using MetarDecoder.ChunkDecoder.Abstract;
using MetarDecoder.Entity;
using MetarDecoder.Exceptions;
using System;
using System.Collections.Generic;

namespace MetarDecoder.ChunkDecoder
{
    public sealed class VisibilityChunkDecoder : MetarChunkDecoder
    {
        public const string CavokParameterName = "Cavok";
        public const string VisibilityParameterName = "Visibility";

        private const string CavokRegexPattern = "CAVOK";
        private const string VisibilityRegexPattern = "([0-9]{4})(NDV)?";
        private const string UsVisibilityRegexPattern = "M?([0-9]{0,2}) ?(([1357])/(2|4|8|16))?SM";
        private const string MinimumVisibilityRegexPattern = "( ([0-9]{4})(N|NE|E|SE|S|SW|W|NW)?)?"; // optional
        private const string NoInfoRegexPattern = "////";

        public override string GetRegex()
        {
            return $"^({CavokRegexPattern}|{VisibilityRegexPattern}{MinimumVisibilityRegexPattern}|{UsVisibilityRegexPattern}|{NoInfoRegexPattern})( )";
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
                throw new MetarChunkDecoderException(remainingMetar, newRemainingMetar, MetarChunkDecoderException.Messages.ForVisibilityInformationBadFormat, this);
            }
            var cavok = false;
            Visibility visibility = null;
            if (found[1].Value == CavokRegexPattern)
            {
                // cloud and visibility OK
                cavok = true;
            }
            else if (found[1].Value == "////")
            {
                // information not available
                cavok = false;
            }
            else
            {
                cavok = false;
                visibility = new Visibility();
                if (!string.IsNullOrEmpty(found[2].Value))
                {
                    // icao visibility
                    visibility.PrevailingVisibility = new Value(Convert.ToDouble(found[2].Value), Value.Unit.Meter);
                    if (!string.IsNullOrEmpty(found[4].Value))
                    {
                        visibility.MinimumVisibility = new Value(Convert.ToDouble(found[5].Value), Value.Unit.Meter);
                        visibility.MinimumVisibilityDirection = found[6].Value;
                    }
                    visibility.NDV = !string.IsNullOrEmpty(found[3].Value);
                }
                else
                {
                    // us visibility
                    double visibilityValue = 0;
                    if (!string.IsNullOrEmpty(found[7].Value))
                    {
                        visibilityValue += Convert.ToInt32(found[7].Value);
                    }
                    if (!string.IsNullOrEmpty(found[9].Value) && !string.IsNullOrEmpty(found[10].Value))
                    {
                        var fractionTop = Convert.ToInt32(found[9].Value);
                        var fractionBottom = Convert.ToInt32(found[10].Value);
                        if (fractionBottom != 0)
                        {
                            visibilityValue += (double)fractionTop / fractionBottom;
                        }
                    }

                    visibility.PrevailingVisibility = new Value(visibilityValue, Value.Unit.StatuteMile);
                }
            }

            result.Add(CavokParameterName, cavok);
            result.Add(VisibilityParameterName, visibility);

            return GetResults(newRemainingMetar, result);
        }
    }
}
