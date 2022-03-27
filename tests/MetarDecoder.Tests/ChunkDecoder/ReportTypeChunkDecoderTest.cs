using MetarDecoder.ChunkDecoder;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using static MetarDecoder.Entity.DecodedMetar;

namespace MetarDecoder.Tests.ChunkDecoder
{
    [TestFixture, Category("ReportTypeChunkDecoder")]
    public class ReportTypeChunkDecoderTest
    {
        private readonly ReportTypeChunkDecoder chunkDecoder = new ReportTypeChunkDecoder();

        /// <summary>
        /// Test parsing of valid report type chunks.
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="type"></param>
        /// <param name="remaining"></param>
        [Test, TestCaseSource("ValidChunks")]
        public void TestParseReportTypeChunk(Tuple<string, MetarType, string> chunk)
        {
            var decoded = chunkDecoder.Parse(chunk.Item1);
            Assert.AreEqual(chunk.Item2, (MetarType)(decoded[Decoder.ResultKey] as Dictionary<string, object>)[ReportTypeChunkDecoder.TypeParameterName]);
            Assert.AreEqual(chunk.Item3, decoded[Decoder.RemainingMetarKey]);
        }

        #region TestCaseSources

        public static List<Tuple<string, MetarType, string>> ValidChunks()
        {
            return new List<Tuple<string, MetarType, string>>() {
                new Tuple<string, MetarType, string>("METAR LFPG", MetarType.METAR, "LFPG"),
                new Tuple<string, MetarType, string>("SPECI LFPB", MetarType.SPECI, "LFPB"),
                new Tuple<string, MetarType, string>("METAR COR LFPO", MetarType.METAR_COR, "LFPO"),
                new Tuple<string, MetarType, string>("SPECI COR PPP", MetarType.SPECI_COR, "PPP"),
                new Tuple<string, MetarType, string>("META LFPG", MetarType.NULL, "META LFPG"),
                new Tuple<string, MetarType, string>("SPECIA LFPG", MetarType.NULL, "SPECIA LFPG"),
                new Tuple<string, MetarType, string>("META COR LFPB", MetarType.NULL, "META COR LFPB"),
                new Tuple<string, MetarType, string>("123 LFPO", MetarType.NULL, "123 LFPO"),
            };
        }
        #endregion
    }
}
