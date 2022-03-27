using MetarDecoder.ChunkDecoder;
using MetarDecoder.Entity;
using NUnit.Framework;
using System.Collections.Generic;

namespace MetarDecoder.Tests.ChunkDecoder
{
    [TestFixture, Category("PresentWeatherChunkDecoder")]
    public class PresentWeatherChunkDecoderTest
    {
        private readonly PresentWeatherChunkDecoder chunkDecoder = new PresentWeatherChunkDecoder();

        /// <summary>
        /// Test parsing of valid recent weather chunks.
        /// </summary>
        /// <param name="chunkToTest"></param>
        [Test, TestCaseSource("ValidChunks")]
        public void TestParsePresentWeatherChunk(PresentWeatherChunkDecoderTester chunkToTest)
        {
            var decoded = chunkDecoder.Parse(chunkToTest.Chunk);
            var pw = (decoded[Decoder.ResultKey] as Dictionary<string, object>)[PresentWeatherChunkDecoder.PresentWeatherParameterName] as List<WeatherPhenomenon>;

            Assert.AreEqual(chunkToTest.NbPhenomens, pw.Count);
            if (chunkToTest.NbPhenomens > 0)
            {
                var phenom1 = pw[0];
                Assert.AreEqual(chunkToTest.intensity1, phenom1.IntensityProximity);
                Assert.AreEqual(chunkToTest.carac1, phenom1.Characteristics);
                Assert.AreEqual(chunkToTest.type1, phenom1.Types);
            }
            if (chunkToTest.NbPhenomens > 1)
            {
                var phenom2 = pw[1];
                Assert.AreEqual(chunkToTest.type2, phenom2.Types);
            }


            Assert.AreEqual(chunkToTest.RemainingMetar, decoded[Decoder.RemainingMetarKey]);
        }

        #region TestCaseSources

        public static List<PresentWeatherChunkDecoderTester> ValidChunks()
        {
            return new List<PresentWeatherChunkDecoderTester>() {
                new PresentWeatherChunkDecoderTester
                {
                    Chunk = "NOTHING HERE",
                    NbPhenomens = 0,
                    intensity1 = null,
                    carac1 = string.Empty,
                    type1 = null,
                    type2 = null,
                    RemainingMetar = "NOTHING HERE",
                },
                new PresentWeatherChunkDecoderTester()
                {
                    Chunk = "FZRA +SN BCFG AAA",
                    NbPhenomens = 3,
                    intensity1 = string.Empty,
                    carac1 = "FZ",
                    type1 = new string[] { "RA" },
                    type2 = new string[] { "SN" },
                    RemainingMetar = "AAA",
                },
                new PresentWeatherChunkDecoderTester()
                {
                    Chunk = "-SG BBB",
                    NbPhenomens = 1,
                    intensity1 = "-",
                    carac1 = string.Empty,
                    type1 = new string[] { "SG" },
                    type2 = null,
                    RemainingMetar = "BBB",
                },
                new PresentWeatherChunkDecoderTester()
                {
                    Chunk = "+GSBRFU VCDRFCPY // CCC",
                    NbPhenomens = 2,
                    intensity1 = "+",
                    carac1 = string.Empty,
                    type1 = new string[] { "GS", "BR", "FU" },
                    type2 = new string[] { "FC", "PY" },
                    RemainingMetar = "CCC",
                },
                new PresentWeatherChunkDecoderTester()
                {
                    Chunk = "// DDD",
                    NbPhenomens = 0,
                    intensity1 = null,
                    carac1 = string.Empty,
                    type1 = null,
                    type2 = null,
                    RemainingMetar = "DDD",
                },
            };
        }

        public class PresentWeatherChunkDecoderTester
        {
            public string Chunk { get; set; }
            public int NbPhenomens { get; set; }
            public string intensity1 { get; set; }
            public string carac1 { get; set; }
            public string[] type1 { get; set; }
            public string[] type2 { get; set; }
            public string RemainingMetar { get; set; }

            public override string ToString()
            {
                return $@"""{Chunk}""";
            }
        }

        #endregion
    }
}
