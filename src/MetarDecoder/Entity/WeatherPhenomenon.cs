using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MetarDecoder.Entity
{
    public sealed class WeatherPhenomenon
    {
        private readonly List<string> _types = new List<string>();

        /// <summary>
        /// Intensity/proximity of the phenomenon + / - / VC (=vicinity)
        /// </summary>
        public string IntensityProximity { get; set; }

        /// <summary>
        /// Characteristics of the phenomenon
        /// </summary>
        public string Characteristics { get; set; }

        /// <summary>
        /// Types of phenomenon
        /// </summary>
        public ReadOnlyCollection<string> Types
        {
            get
            {
                return new ReadOnlyCollection<string>(_types);
            }
        }

        public void AddType(string type)
        {
            _types.Add(type);
        }
    }
}