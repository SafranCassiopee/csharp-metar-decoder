using MetarDecoder.ChunkDecoder;
using MetarDecoder.Exceptions;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MetarDecoder.Tests.ChunkDecoder
{
    [TestFixture, Category("DatetimeChunkDecoder")]
    public class DatetimeChunkDecoderTest
    {
        private readonly DatetimeChunkDecoder chunkDecoder = new DatetimeChunkDecoder();

        /// <summary>
        /// Test parsing of valid icao chunks.
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="icao"></param>
        /// <param name="remaining"></param>
        [Test, TestCaseSource("ValidChunks")]
        public void TestParseDatetimeChunk(Tuple<string, int, string, string> chunk)
        {
            var decoded = new Dictionary<string, object>();
            Assert.DoesNotThrow(() =>
            {
                decoded = chunkDecoder.Parse(chunk.Item1);
            });

            Assert.IsTrue(decoded.ContainsKey(Decoder.ResultKey));

            //check Day
            Assert.IsTrue(((Dictionary<string, object>)decoded[Decoder.ResultKey]).ContainsKey(DatetimeChunkDecoder.DayParameterName));
            Assert.NotNull(((Dictionary<string, object>)decoded[Decoder.ResultKey])[DatetimeChunkDecoder.DayParameterName]);
            Assert.AreEqual(chunk.Item2, (int)((Dictionary<string, object>)decoded[Decoder.ResultKey])[DatetimeChunkDecoder.DayParameterName]);

            //check Time
            Assert.IsTrue(((Dictionary<string, object>)decoded[Decoder.ResultKey]).ContainsKey(DatetimeChunkDecoder.TimeParameterName));
            Assert.NotNull(((Dictionary<string, object>)decoded[Decoder.ResultKey])[DatetimeChunkDecoder.TimeParameterName]);
            Assert.AreEqual(chunk.Item3 + " UTC", ((Dictionary<string, object>)decoded[Decoder.ResultKey])[DatetimeChunkDecoder.TimeParameterName] as string);

            //check RemainingMetar
            Assert.IsTrue(decoded.ContainsKey(Decoder.RemainingMetarKey));
            Assert.AreEqual(chunk.Item4, decoded[Decoder.RemainingMetarKey] as string);
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
        public static List<Tuple<string, int, string, string>> ValidChunks()
        {
            return new List<Tuple<string, int, string, string>>()
            {
                new Tuple<string, int, string, string>("271035Z AAA", 27, "10:35", "AAA"),
                new Tuple<string, int, string, string>("012342Z BBB", 1,  "23:42", "BBB"),
                new Tuple<string, int, string, string>("311200Z CCC", 31, "12:00", "CCC"),
            };
        }

        public static List<string> InvalidChunks()
        {
            return new List<string>() {
                "271035 AAA",
                "2102Z AAA",
                "123580Z AAA",
                "122380Z AAA",
                "351212Z AAA",
                "35018Z AAA",
            };
        }
        #endregion
    }
}
