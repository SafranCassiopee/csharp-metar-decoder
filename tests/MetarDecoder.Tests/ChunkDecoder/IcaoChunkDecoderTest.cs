using MetarDecoder.ChunkDecoder;
using MetarDecoder.Exceptions;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MetarDecoder.Tests.ChunkDecoder
{
    [TestFixture, Category("IcaoChunkDecoder")]
    public class IcaoChunkDecoderTest
    {
        private readonly IcaoChunkDecoder chunkDecoder = new IcaoChunkDecoder();

        /// <summary>
        /// Test parsing of valid icao chunks.
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="icao"></param>
        /// <param name="remaining"></param>
        [Test, TestCaseSource("ValidChunks")]
        public void TestParseIcaoChunk(Tuple<string, string, string> chunk)
        {
            var decoded = new Dictionary<string, object>();
            Assert.DoesNotThrow(() =>
            {
                decoded = chunkDecoder.Parse(chunk.Item1);
            });

            Assert.IsTrue(decoded.ContainsKey(Decoder.ResultKey));

            //check ICAO
            Assert.AreEqual(chunk.Item2, ((Dictionary<string, object>)decoded[Decoder.ResultKey])[IcaoChunkDecoder.ICAOParameterName] as string);

            //check RemainingMetar
            Assert.AreEqual(chunk.Item3, decoded[Decoder.RemainingMetarKey] as string);
        }

        /// <summary>
        /// Test parsing of invalid icao chunks.
        /// </summary>
        /// <param name="chunk"></param>
        /// 
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
            return new List<Tuple<string, string, string>>()
            {
                new Tuple<string, string, string>("LFPG AAA", "LFPG", "AAA"),
                new Tuple<string, string, string>("LFPO BBB", "LFPO", "BBB"),
                new Tuple<string, string, string>("LFIO CCC", "LFIO", "CCC"),
            };
        }

        public static List<string> InvalidChunks()
        {
            return new List<string>() {
                "LFA AAA",
                "L AAA",
                "LFP BBB",
                "LF8 CCC" };
        }
        #endregion
    }
}
