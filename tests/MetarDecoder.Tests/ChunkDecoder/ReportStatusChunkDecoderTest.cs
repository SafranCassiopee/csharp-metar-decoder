using MetarDecoder.ChunkDecoder;
using MetarDecoder.Exceptions;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MetarDecoder.Tests.ChunkDecoder
{
    [TestFixture, Category("ReportStatusChunkDecoder")]
    public class ReportStatusChunkDecoderTest
    {
        private readonly ReportStatusChunkDecoder chunkDecoder = new ReportStatusChunkDecoder();

        /// <summary>
        /// Test parsing of valid report type chunks.
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="type"></param>
        /// <param name="remaining"></param>
        [Test, TestCaseSource("ValidChunks")]
        public void TestParseReportStatusChunk(Tuple<string, string, string> chunk)
        {
            var decoded = chunkDecoder.Parse(chunk.Item1);
            Assert.AreEqual(chunk.Item2, (decoded[Decoder.ResultKey] as Dictionary<string, object>)[ReportStatusChunkDecoder.StatusParameterName] as string);
            Assert.AreEqual(chunk.Item3, decoded[Decoder.RemainingMetarKey]);
        }

        /// <summary>
        /// Test parsing of invalid report status chunks.
        /// </summary>
        /// <param name=""></param>
        [Test, TestCaseSource("InvalidChunks")]
        public void TestParseInvalidChunk(string chunk)
        {
            var decoded = new Dictionary<string, object>();
            var ex = Assert.Throws(typeof(MetarChunkDecoderException), () =>
            {
                decoded = chunkDecoder.Parse(chunk);
            }) as MetarChunkDecoderException;
            Assert.IsFalse(decoded.ContainsKey(Decoder.ResultKey));
            Assert.AreEqual(chunk, ex.RemainingMetar);
        }

        #region TestCaseSources

        public static List<Tuple<string, string, string>> ValidChunks()
        {
            return new List<Tuple<string, string, string>>() {
                new Tuple<string, string, string>("NIL ", "NIL", string.Empty),
                new Tuple<string, string, string>("AUTO AAA", "AUTO", "AAA"),
                new Tuple<string, string, string>("AUTO AUTO", "AUTO", "AUTO"),
                new Tuple<string, string, string>("COR BBB", "COR", "BBB"),
                new Tuple<string, string, string>("1234 CCC", string.Empty, "1234 CCC"),
                new Tuple<string, string, string>("AFK DDD", "AFK", "DDD"),
            };
        }
        public static List<string> InvalidChunks
        {
            get
            {
                return new List<string>()
                {
                    "NIL AAA", "NIL NIL", "AUTIO BBB", "AU CCC", "R DDD"
                };
            }
        }
        #endregion
    }
}
