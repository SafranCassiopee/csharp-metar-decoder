using MetarDecoder.Exceptions;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MetarDecoder.Entity
{
    public sealed class DecodedMetar
    {
        public enum MetarType
        {
            NULL,
            METAR,
            METAR_COR,
            SPECI,
            SPECI_COR,
        }
        public enum MetarStatus
        {
            NULL,
            AUTO,
            NIL,
        }

        private string _rawMetar;
        /// <summary>
        /// Raw METAR
        /// </summary>
        public string RawMetar
        {
            get
            {
                return _rawMetar.Trim();
            }
            set
            {
                _rawMetar = value;
            }
        }

        /// <summary>
        /// Decoding exceptions, if any
        /// </summary>
        private List<MetarChunkDecoderException> _decodingExceptions = new List<MetarChunkDecoderException>();

        /// <summary>
        /// If the decoded metar is invalid, get all the exceptions that occurred during decoding
        /// Note that in strict mode, only the first encountered exception will be reported as parsing stops on error
        /// Else return null;.
        /// </summary>
        public ReadOnlyCollection<MetarChunkDecoderException> DecodingExceptions
        {
            get
            {
                return new ReadOnlyCollection<MetarChunkDecoderException>(_decodingExceptions);
            }
        }

        /// <summary>
        /// Report type (METAR, METAR COR or SPECI)
        /// </summary>
        public MetarType Type { get; set; } = MetarType.NULL;

        /// <summary>
        /// ICAO code of the airport where the observation has been made
        /// </summary>
        public string ICAO { get; set; } = string.Empty;

        /// <summary>
        /// Day of this observation
        /// </summary>
        public int? Day { get; set; }

        /// <summary>
        /// Time of the observation, as a string
        /// </summary>
        public string Time { get; set; } = string.Empty;

        /// <summary>
        /// Report status (AUTO or NIL)
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Surface wind information
        /// </summary>
        public SurfaceWind SurfaceWind { get; set; }

        /// <summary>
        /// Visibility information
        /// </summary>
        public Visibility Visibility { get; set; }

        public bool Cavok { get; set; } = false;

        /// <summary>
        /// Runway visual range information
        /// </summary>
        public List<RunwayVisualRange> RunwaysVisualRange { get; set; } = new List<RunwayVisualRange>();

        /// <summary>
        /// Present weather
        /// </summary>
        public List<WeatherPhenomenon> PresentWeather { get; set; } = new List<WeatherPhenomenon>();

        /// <summary>
        /// Cloud layers information
        /// </summary>
        public List<CloudLayer> Clouds { get; set; } = new List<CloudLayer>();

        /// <summary>
        /// Temperature information
        /// </summary>
        public Value AirTemperature { get; set; }

        /// <summary>
        /// Temperature information
        /// </summary>
        public Value DewPointTemperature { get; set; }

        /// <summary>
        /// Pressure information
        /// </summary>
        public Value Pressure { get; set; }

        /// <summary>
        /// Recent weather
        /// </summary>
        public WeatherPhenomenon RecentWeather { get; set; }

        /// <summary>
        /// Windshear runway information (which runways, or "all")
        /// </summary>
        public bool? WindshearAllRunways { get; set; }

        /// <summary>
        /// Windshear runway information (which runways, or "all")
        /// </summary>
        public List<string> WindshearRunways { get; set; }

        internal DecodedMetar(string rawMetar = "")
        {
            RawMetar = rawMetar;
        }

        /// <summary>
        /// Reset the whole list of Decoding Exceptions
        /// </summary>
        public void ResetDecodingExceptions()
        {
            _decodingExceptions = new List<MetarChunkDecoderException>();
        }

        /// <summary>
        /// Check if the decoded metar is valid, i.e. if there was no error during decoding.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return DecodingExceptions.Count == 0;
            }
        }

        /// <summary>
        /// Add an exception that occured during metar decoding.
        /// </summary>
        /// <param name="ex"></param>
        public void AddDecodingException(MetarChunkDecoderException ex)
        {
            _decodingExceptions.Add(ex);
        }
    }
}
