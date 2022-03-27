using MetarDecoder.ChunkDecoder;
using MetarDecoder.Entity;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MetarDecoder.Tests.ChunkDecoder
{
    [TestFixture, Category("RecentWeatherChunkDecoder")]
    public class RecentWeatherChunkDecoderTest
    {
        private readonly RecentWeatherChunkDecoder chunkDecoder = new RecentWeatherChunkDecoder();

        /// <summary>
        /// Test parsing of valid recent weather chunks.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        [Test, TestCaseSource("ValidChunks")]
        public void TestParseRecentWeatherChunk(Tuple<string, string, string[], string> chunkToTest)
        {
            var decoded = chunkDecoder.Parse(chunkToTest.Item1);
            var recent = (decoded[Decoder.ResultKey] as Dictionary<string, object>)[RecentWeatherChunkDecoder.RecentWeatherParameterName] as WeatherPhenomenon;
            Assert.AreEqual(chunkToTest.Item2, recent.Characteristics);
            Assert.AreEqual(chunkToTest.Item3, recent.Types);
            Assert.AreEqual(chunkToTest.Item4, decoded[Decoder.RemainingMetarKey]);
        }

        #region TestCaseSources

        public static List<Tuple<string, string, string[], string>> ValidChunks()
        {
            return new List<Tuple<string, string, string[], string>>()
            {
                new Tuple<string, string, string[], string>("REBLSN AAA",   "BL",   new string[] { "SN" },          "AAA"),
                new Tuple<string, string, string[], string>("REPL BBB",     "",     new string[] { "PL" },          "BBB"),
                new Tuple<string, string, string[], string>("RETSRA CCC",   "TS",   new string[] { "RA" },          "CCC"),
                new Tuple<string, string, string[], string>("RETSRABR DDD", "TS",   new string[] { "RA", "BR" },    "DDD"),
            };
        }
        #endregion
    }
}
