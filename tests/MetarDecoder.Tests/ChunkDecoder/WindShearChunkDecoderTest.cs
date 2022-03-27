using MetarDecoder.ChunkDecoder;
using MetarDecoder.Exceptions;
using NUnit.Framework;
using System.Collections.Generic;

namespace MetarDecoder.Tests.ChunkDecoder
{
    [TestFixture, Category("WindShearChunkDecoder")]
    public class WindShearChunkDecoderTest
    {
        private readonly WindShearChunkDecoder chunkDecoder = new WindShearChunkDecoder();

        /// <summary>
        /// Test parsing of valid windshear chunks.
        /// </summary>
        /// <param name="chunkToTest"></param>
        [Test, TestCaseSource("ValidChunks")]
        public void TestParseWindShearChunk(WindShearChunkDecoderTester chunkToTest)
        {
            var decoded = chunkDecoder.Parse(chunkToTest.Chunk);
            var result = decoded[Decoder.ResultKey] as Dictionary<string, object>;
            Assert.AreEqual(chunkToTest.AllRunways, (bool)result[WindShearChunkDecoder.WindshearAllRunwaysParameterName]);
            Assert.AreEqual(chunkToTest.Runways, result[WindShearChunkDecoder.WindshearRunwaysParameterName] as List<string>);
            Assert.AreEqual(chunkToTest.RemainingMetar, decoded[Decoder.RemainingMetarKey]);
        }

        /// <summary>
        /// Test parsing of invalid wind shear chunks (bad format).
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [Test, TestCaseSource("BadFormatChunk")]
        public void TestParseBadFormatChunk(string chunk)
        {
            //need fine tuning
            var decoded = chunkDecoder.Parse(chunk);
            var res = decoded[Decoder.ResultKey];
            Assert.AreEqual(chunk, decoded[Decoder.RemainingMetarKey]);
        }

        /// <summary>
        /// Test parsing of invalid wind shear chunks (invalid QFU).
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
        public static List<WindShearChunkDecoderTester> ValidChunks()
        {
            return new List<WindShearChunkDecoderTester>()
            {
                new WindShearChunkDecoderTester()
                {
                    Chunk = "WS R03L WS R32 WS R25C AAA",
                    AllRunways = false,
                    Runways = new string[] { "03L", "32", "25C" },
                    RemainingMetar = "AAA",
                },
                new WindShearChunkDecoderTester()
                {
                    Chunk = "WS R18C BBB",
                    AllRunways = false,
                    Runways = new string[] { "18C" },
                    RemainingMetar = "BBB",
                },
                new WindShearChunkDecoderTester()
                {
                    Chunk = "WS ALL RWY CCC",
                    AllRunways = true,
                    Runways = null,
                    RemainingMetar = "CCC",
                },
                new WindShearChunkDecoderTester()
                {
                    Chunk = "WS RWY22 DDD",
                    AllRunways = false,
                    Runways = new string[] { "22" },
                    RemainingMetar = "DDD",
                },
            };
        }

        public class WindShearChunkDecoderTester
        {
            public string Chunk { get; set; }
            public bool AllRunways { get; set; }
            public string[] Runways { get; set; }
            public string RemainingMetar { get; set; }

            public override string ToString()
            {
                return $@"""{Chunk}""";
            }
        }

        public static List<string> BadFormatChunk()
        {
            return new List<string>()
            {
                "W RWY AAA", "WS ALL BBB", "WS R12P CCC"
            };
        }
        public static List<string> InvalidChunks()
        {
            return new List<string>()
            {
                "WS RWY00 AAA", "WS R40 BBB", "WS R50C CCC"
            };
        }

        #endregion

    }
}
