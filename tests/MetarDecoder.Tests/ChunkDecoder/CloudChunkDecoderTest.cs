using MetarDecoder.ChunkDecoder;
using MetarDecoder.Entity;
using MetarDecoder.Exceptions;
using NUnit.Framework;
using System.Collections.Generic;
using static MetarDecoder.Entity.CloudLayer;

namespace MetarDecoder.Tests.ChunkDecoder
{
    [TestFixture, Category("CloudChunkDecoder")]
    public class CloudChunkDecoderTest
    {
        private readonly CloudChunkDecoder chunkDecoder = new CloudChunkDecoder();

        /// <summary>
        /// Test parsing of valid cloud chunks.
        /// </summary>
        /// <param name="chunkToTest"></param>
        [Test, TestCaseSource("ValidChunks")]
        public void TestParseCloudChunk(CloudChunkDecoderTester chunkToTest)
        {
            var decoded = chunkDecoder.Parse(chunkToTest.Chunk);
            var clouds = (decoded[Decoder.ResultKey] as Dictionary<string, object>)[CloudChunkDecoder.CloudsParameterName] as List<CloudLayer>;

            Assert.AreEqual(chunkToTest.nbLayers, clouds.Count);

            if (clouds.Count > 0)
            {
                var cloud = clouds[0];
                Assert.AreEqual(chunkToTest.layer1Amount, cloud.Amount);
                if (chunkToTest.layer1BaseHeight != null)
                {
                    Assert.AreEqual(chunkToTest.layer1BaseHeight, cloud.BaseHeight.ActualValue);
                    Assert.AreEqual(Value.Unit.Feet, cloud.BaseHeight.ActualUnit);
                }
                else
                {
                    Assert.IsNull(cloud.BaseHeight);
                }
                Assert.AreEqual(chunkToTest.layer1Type, cloud.Type);
            }
            Assert.AreEqual(chunkToTest.RemainingMetar, decoded[Decoder.RemainingMetarKey]);
        }

        [Test, TestCaseSource("InvalidChunks")]
        public void TestParseCAVOKChunk(string chunk)
        {
            var decoded = chunkDecoder.Parse(chunk, true);
            Assert.AreEqual(0, ((decoded[Decoder.ResultKey] as Dictionary<string, object>)[CloudChunkDecoder.CloudsParameterName] as List<CloudLayer>).Count);
        }

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

        public static List<CloudChunkDecoderTester> ValidChunks()
        {
            return new List<CloudChunkDecoderTester>()
            {
                new CloudChunkDecoderTester()
                {
                    Chunk = "VV085 AAA",
                    nbLayers = 1,
                    layer1Amount = CloudAmount.VV,
                    layer1BaseHeight = 8500,
                    layer1Type = CloudType.NULL,
                    RemainingMetar = "AAA"
                },
                new CloudChunkDecoderTester()
                {
                    Chunk = "BKN200TCU OVC250 VV/// BBB",
                    nbLayers = 3,
                    layer1Amount = CloudAmount.BKN,
                    layer1BaseHeight = 20000,
                    layer1Type = CloudType.TCU,
                    RemainingMetar = "BBB"
                },
                new CloudChunkDecoderTester()
                {
                    Chunk = "OVC////// FEW250 CCC",
                    nbLayers = 2,
                    layer1Amount = CloudAmount.OVC,
                    layer1BaseHeight = null,
                    layer1Type = CloudType.CannotMeasure,
                    RemainingMetar = "CCC"
                },
                new CloudChunkDecoderTester()
                {
                    Chunk = "OVC////// SCT250 CCC",
                    nbLayers = 2,
                    layer1Amount = CloudAmount.OVC,
                    layer1BaseHeight = null,
                    layer1Type = CloudType.CannotMeasure,
                    RemainingMetar = "CCC"
                },
                new CloudChunkDecoderTester()
                {
                    Chunk = "NSC DDD",
                    nbLayers = 0,
                    layer1Amount = CloudAmount.NULL,
                    layer1BaseHeight = null,
                    layer1Type = CloudType.NULL,
                    RemainingMetar = "DDD"
                },
                new CloudChunkDecoderTester()
                {
                    Chunk = "SKC EEE",
                    nbLayers = 0,
                    layer1Amount = CloudAmount.NULL,
                    layer1BaseHeight = null,
                    layer1Type = CloudType.NULL,
                    RemainingMetar = "EEE"
                },
                new CloudChunkDecoderTester()
                {
                    Chunk = "NCD FFF",
                    nbLayers = 0,
                    layer1Amount = CloudAmount.NULL,
                    layer1BaseHeight = null,
                    layer1Type = CloudType.NULL,
                    RemainingMetar = "FFF"
                },
            };
        }

        public class CloudChunkDecoderTester
        {
            public string Chunk { get; set; }
            public int nbLayers { get; set; }
            public CloudAmount layer1Amount { get; set; }
            public int? layer1BaseHeight { get; set; }
            public CloudType layer1Type { get; set; }
            public string RemainingMetar { get; set; }

            public override string ToString()
            {
                return $@"""{Chunk}""";
            }
        }

        public static List<string> InvalidChunks()
        {
            return new List<string> {
                "FEW10 EEE",
                "AAA EEE",
                "BKN100A EEE",
            };
        }
        #endregion

    }
}
