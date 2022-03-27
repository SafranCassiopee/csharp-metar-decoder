using MetarDecoder.ChunkDecoder.Abstract;
using MetarDecoder.Entity;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MetarDecoder.Tests
{
    [TestFixture]
    public class BasicTest
    {
        public readonly ReadOnlyCollection<string> TestMetarSource = new ReadOnlyCollection<string>(new List<string>() {
            "LFPO 231027Z AUTO 24004G09MPS 2500 1000NW R32/0400 R08C/0004D +FZRA VCSN //FEW015 17/10 Q1009 REFZRA WS R03",
        });

        List<DecodedMetar> DecodedMetars;

        [SetUp]
        public void Setup()
        {
            DecodedMetars = TestMetarSource.Select(metar => Decoder.ParseWithMode(metar)).ToList();
        }

        [Test, Category("Basic")]
        public void RunToCompletionTest()
        {
            Assert.IsNotNull(DecodedMetars[0]);
        }

        [Test, Category("Basic")]
        public void CheckRawMetarNotNull()
        {
            Assert.AreEqual(TestMetarSource[0], DecodedMetars[0].RawMetar);
        }

        [Test, Category("Basic")]
        public void ConsumeOneChunkTest()
        {
            var result = MetarChunkDecoder.ConsumeOneChunk(TestMetarSource[0]);

            Assert.AreEqual("231027Z AUTO 24004G09MPS 2500 1000NW R32/0400 R08C/0004D +FZRA VCSN //FEW015 17/10 Q1009 REFZRA WS R03", result);
        }
    }
}
