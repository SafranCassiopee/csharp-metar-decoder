using MetarDecoder.ChunkDecoder.Abstract;
using MetarDecoder.Exceptions;
using System;
using System.Collections.Generic;

namespace MetarDecoder.ChunkDecoder
{
    public sealed class DatetimeChunkDecoder : MetarChunkDecoder
    {
        public const string DayParameterName = "Day";
        public const string TimeParameterName = "Time";

        public override string GetRegex()
        {
            return "^([0-9]{2})([0-9]{2})([0-9]{2})Z ";
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
                throw new MetarChunkDecoderException(remainingMetar, newRemainingMetar, MetarChunkDecoderException.Messages.BadDayHourMinuteInformation, this);
            }

            // retrieve found params and check them
            var day = Convert.ToInt32(found[1].Value);
            var hour = Convert.ToInt32(found[2].Value);
            var minute = Convert.ToInt32(found[3].Value);

            if (!checkValidity(day, hour, minute))
            {
                throw new MetarChunkDecoderException(remainingMetar, newRemainingMetar, MetarChunkDecoderException.Messages.InvalidDayHourMinuteRanges, this);
            }

            result.Add(DayParameterName, day);
            result.Add(TimeParameterName, $"{hour:00}:{minute:00} UTC");

            return GetResults(newRemainingMetar, result);
        }

        /// <summary>
        /// Check the validity of the decoded information for date time.
        /// </summary>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <returns></returns>
        private bool checkValidity(int day, int hour, int minute)
        {
            // check value range
            if (day < 1 || day > 31)
            {
                return false;
            }
            if (hour < 0 || hour > 23)
            {
                return false;
            }
            if (minute < 0 || minute > 59)
            {
                return false;
            }
            return true;
        }
    }
}
