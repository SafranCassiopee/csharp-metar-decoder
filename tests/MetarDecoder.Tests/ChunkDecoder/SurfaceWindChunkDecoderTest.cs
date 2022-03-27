using MetarDecoder.ChunkDecoder;
using MetarDecoder.Entity;
using MetarDecoder.Exceptions;
using NUnit.Framework;
using System.Collections.Generic;

namespace MetarDecoder.Tests.ChunkDecoder
{
    [TestFixture, Category("SurfaceWindChunkDecoder")]
    public class SurfaceWindChunkDecoderTest
    {
        private readonly SurfaceWindChunkDecoder chunkDecoder = new SurfaceWindChunkDecoder();

        /// <summary>
        /// Test parsing of valid surface wind chunks.
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="direction"></param>
        /// <param name="variable_direction"></param>
        /// <param name="speed"></param>
        /// <param name="speed_variations"></param>
        /// <param name="speed_unit"></param>
        /// <param name="direction_variations"></param>
        /// <param name="remaining"></param>
        [Test, TestCaseSource("ValidChunks")]
        public void TestParseSurfaceWindChunk(ValidSurfaceWindChunkDecoderTester chunkToTest)
        {
            var decoded = chunkDecoder.Parse(chunkToTest.Chunk);
            Assert.IsNotNull(decoded);
            var wind = (decoded[Decoder.ResultKey] as Dictionary<string, object>)[SurfaceWindChunkDecoder.SurfaceWindParameterName] as SurfaceWind;
            if (!chunkToTest.VariableDirection)
            {
                Assert.AreEqual(chunkToTest.Direction, wind.MeanDirection.ActualValue);
                Assert.AreEqual(Value.Unit.Degree, wind.MeanDirection.ActualUnit);
            }
            Assert.AreEqual(chunkToTest.VariableDirection, wind.VariableDirection);
            if (chunkToTest.DirectionVariations != null)
            {
                Assert.AreEqual(chunkToTest.DirectionVariations[0], wind.DirectionVariations[0].ActualValue);
                Assert.AreEqual(chunkToTest.DirectionVariations[1], wind.DirectionVariations[1].ActualValue);
                Assert.AreEqual(Value.Unit.Degree, wind.DirectionVariations[0].ActualUnit);
            }
            Assert.AreEqual(chunkToTest.Speed, wind.MeanSpeed.ActualValue);
            if (chunkToTest.SpeedVariations.HasValue)
            {
                Assert.AreEqual(chunkToTest.SpeedVariations, wind.SpeedVariations.ActualValue);
            }
            Assert.AreEqual(chunkToTest.SpeedUnit, wind.MeanSpeed.ActualUnit);
            Assert.AreEqual(chunkToTest.RemainingMetar, decoded[Decoder.RemainingMetarKey]);
        }

        /// <summary>
        /// Test parsing of invalid surface wind chunks.
        /// </summary>
        /// <param name="chunk">chunk to test</param>
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

        /// <summary>
        /// Test parsing of chunk with no information.
        /// </summary>
        [Test]
        public void TestEmptyInformationChunk()
        {
            var ex = Assert.Throws(typeof(MetarChunkDecoderException), () =>
            {
                chunkDecoder.Parse("/////KT PPP");
            }) as MetarChunkDecoderException;
            Assert.AreEqual("PPP", ex.NewRemainingMetar);
        }

        #region TestCaseSources

        public static List<ValidSurfaceWindChunkDecoderTester> ValidChunks()
        {
            return new List<ValidSurfaceWindChunkDecoderTester>()
            {
                new ValidSurfaceWindChunkDecoderTester()
                {
                    Chunk = "VRB01MPS AAA",
                    Direction = null,
                    VariableDirection = true,
                    Speed = 1,
                    SpeedVariations = null,
                    SpeedUnit = Value.Unit.MeterPerSecond,
                    DirectionVariations = null,
                    RemainingMetar = "AAA"
                },
                new ValidSurfaceWindChunkDecoderTester()
                {
                   Chunk = "24004MPS BBB",
                    Direction = 240,
                    VariableDirection = false,
                    Speed = 4,
                    SpeedVariations = null,
                    SpeedUnit = Value.Unit.MeterPerSecond,
                    DirectionVariations = null,
                    RemainingMetar = "BBB"
                },
                new ValidSurfaceWindChunkDecoderTester()
                {
                  Chunk = "140P99KT CCC",
                    Direction = 140,
                    VariableDirection =false,
                    Speed=99,
                    SpeedVariations = null,
                    SpeedUnit = Value.Unit.Knot,
                    DirectionVariations = null,
                    RemainingMetar = "CCC"
                },
                new ValidSurfaceWindChunkDecoderTester()
                {
                    Chunk = "02005MPS 350V070 DDD",
                    Direction = 20,
                    VariableDirection =false,
                    Speed  =5,
                    SpeedVariations = null,
                    SpeedUnit = Value.Unit.MeterPerSecond,
                    DirectionVariations = new int[] {350, 70 },
                    RemainingMetar = "DDD"
                },
                new ValidSurfaceWindChunkDecoderTester()
                {
                  Chunk =  "12003KPH FFF",
                    Direction = 120,
                    VariableDirection =false,
                    Speed = 3,
                    SpeedVariations = null,
                    SpeedUnit = Value.Unit.KilometerPerHour,
                    DirectionVariations = null,
                    RemainingMetar =  "FFF"
                },
            };
        }

        public class ValidSurfaceWindChunkDecoderTester
        {
            public string Chunk { get; set; }
            public int? Direction { get; set; }
            public bool VariableDirection { get; set; }
            public double Speed { get; set; }
            public int? SpeedVariations { get; set; }
            public Value.Unit SpeedUnit { get; set; }
            public int[] DirectionVariations { get; set; }
            public string RemainingMetar { get; set; }

            public override string ToString()
            {
                return $@"""{Chunk}""";
            }
        }

        public static List<string> InvalidChunks()
        {
            return new List<string> {
                "12003G09 AAA",
                "VRB01MP BBB",
                "38003G12MPS CCC",
                "12003KPA DDD",
                "02005MPS 450V070 EEE",
                "02005MPS 110V600 FFF"
            };
        }
        #endregion
    }
}
