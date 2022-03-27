namespace MetarDecoder.Entity
{
    public sealed class RunwayVisualRange
    {
        public enum Tendency
        {
            NONE,
            U,
            D,
            N,
        }

        /// <summary>
        /// Concerned runway
        /// </summary>
        public string Runway { get; set; }

        /// <summary>
        /// Visual range defined by one value
        /// </summary>
        public Value VisualRange { get; set; }

        /// <summary>
        /// Visual range defined by an interval (because it is variable)
        /// </summary>
        public Value[] VisualRangeInterval { get; set; }

        /// <summary>
        /// Is it a variable range ?
        /// </summary>
        public bool Variable { get; set; }

        /// <summary>
        /// Past tendency (optional) (U, D, or N)
        /// </summary>
        public Tendency PastTendency { get; set; }
    }
}