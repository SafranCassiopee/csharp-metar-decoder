using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MetarDecoder.Entity
{
    [DebuggerDisplay("{ActualValue} {ActualUnit}")]
    public sealed class Value
    {
        public double ActualValue { get; private set; }
        public Unit ActualUnit { get; private set; }

        public enum Unit
        {
            [Description("")]
            None,
            [Description("deg C")]
            DegreeCelsius,
            [Description("deg")]
            Degree,
            [Description("kt")]
            Knot,
            [Description("m/s")]
            MeterPerSecond,
            [Description("km/h")]
            KilometerPerHour,
            [Description("m")]
            Meter,
            [Description("ft")]
            Feet,
            [Description("SM")]
            StatuteMile,
            [Description("hPa")]
            HectoPascal,
            [Description("inHg")]
            MercuryInch,
            [Description("N/A")]
            UnknownUnit,
        }

        /// <summary>
        /// Conversions maps, internal usage only
        /// </summary>
        private readonly Dictionary<Unit, Dictionary<Unit, double>> ConversionMaps = new Dictionary<Unit, Dictionary<Unit, double>>() {
            {
                Unit.MeterPerSecond, new Dictionary<Unit, double>()
                {
                    { Unit.MeterPerSecond, 1f  },
                    { Unit.KilometerPerHour, 0.277778f },
                    { Unit.Knot, 0.51444f },
                }
            },
            {
                Unit.Meter, new Dictionary<Unit, double>()
                {
                    { Unit.Meter, 1f  },
                    { Unit.Feet, 0.3048f },
                    { Unit.StatuteMile, 1609.34f },
                }
            },
            {
                Unit.HectoPascal, new Dictionary<Unit, double>()
                {
                    { Unit.HectoPascal, 1f  },
                    { Unit.MercuryInch, 33.86389f },
                }
            }


        };
        public Value(double value, Unit unit)
        {
            ActualValue = value;
            ActualUnit = unit;
        }

        /// <summary>
        /// Returns converted value of unit.
        /// </summary>
        /// <param name="unitTo"></param>
        /// <returns></returns>
        public float GetConvertedValue(Unit unitTo)
        {
            var rateFrom = GetConversionRate(ActualUnit);
            var rateTo = GetConversionRate(unitTo);
            return (float)Math.Round((ActualValue * rateFrom) / rateTo, 3);
        }

        /// <summary>
        /// Returns conversion rate between original METAR unit and requested unit.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        private double GetConversionRate(Unit unit)
        {
            var conversionMap = GetConversionMap();
            if (!conversionMap.Value.ContainsKey(unit))
            {
                throw new ArgumentException($@"Conversion rate between ""{unit}"" and ""{ActualUnit}"" is not defined.");
            }
            return conversionMap.Value[unit];
        }

        /// <summary>
        /// Returns conversion map based on original METAR unit.
        /// </summary>
        /// <returns></returns>
        private KeyValuePair<Unit, Dictionary<Unit, double>> GetConversionMap()
        {
            foreach (var conversionMap in ConversionMaps)
            {
                if (conversionMap.Value.ContainsKey(ActualUnit))
                {
                    return conversionMap;
                }
            }
            throw new ArgumentException("Trying to convert unsupported values");
        }

        /// <summary>
        /// Convert a string value into an int, and takes into account some non-numeric char
        /// P = +, M = -, / = null
        /// </summary>
        public static int? ToInt(string value)
        {
            //warning: Regex.Replace is not in PHP version
            var valueNumeric = Regex.Replace(value.Replace("P", "").Replace("M", "-"), "[A-Z]", string.Empty);
            if (Regex.Match(valueNumeric, @"^[\-0-9]").Success)
            {
                return Convert.ToInt32(valueNumeric);
            }
            return null;
        }

        public override string ToString()
        {
            return $"{ActualValue} {ActualUnit}";
        }
    }
}
