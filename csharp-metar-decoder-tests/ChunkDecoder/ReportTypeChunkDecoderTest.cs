using NUnit.Framework;
using System.Collections.Generic;
using csharp_metar_decoder;
using csharp_metar_decoder.entity;
using csharp_metar_decoder.chunkdecoder;
using System;
using static csharp_metar_decoder.entity.DecodedMetar;

namespace csharp_metar_decoder_tests.chunkdecoder
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
            Assert.AreEqual(chunk.Item2, (MetarType)((decoded[MetarDecoder.ResultKey] as Dictionary<string, object>)[ReportTypeChunkDecoder.TypeParameterName]));
            Assert.AreEqual(chunk.Item3, decoded[MetarDecoder.RemainingMetarKey]);
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
