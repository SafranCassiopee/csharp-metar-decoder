using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static MetarDecoder.Entity.CloudLayer;
using static MetarDecoder.Entity.DecodedMetar;
using static MetarDecoder.Entity.RunwayVisualRange;
using static MetarDecoder.Entity.Value;

namespace MetarDecoder.Tests
{
    [TestFixture, Category("MetarDecoderTest")]
    public class MetarDecoderTest
    {
        private Decoder decoder;
        [SetUp]
        public void Setup()
        {
            decoder = new Decoder();
        }

        /// <summary>
        /// Test parsing of a complete, valid METAR.
        /// </summary>
        ///         [Test]
        [Test]
        public void TestParse()
        {
            // launch decoding
            var raw_metar = "METAR  LFPO 231027Z   AUTO 24004G09MPS 2500 1000NW R32/0400 R08C/0004D +FZRA VCSN // FEW015 17/10 Q1009 REFZRA WS R03";
            var d = decoder.ParseStrict(raw_metar);
            // compare results
            Assert.IsTrue(d.IsValid);
            Assert.AreEqual("METAR LFPO 231027Z AUTO 24004G09MPS 2500 1000NW R32/0400 R08C/0004D +FZRA VCSN // FEW015 17/10 Q1009 REFZRA WS R03", d.RawMetar);
            Assert.AreEqual(MetarType.METAR, d.Type);
            Assert.AreEqual("LFPO", d.ICAO);
            Assert.AreEqual(23, d.Day);
            Assert.AreEqual("10:27 UTC", d.Time);
            Assert.AreEqual("AUTO", d.Status);
            var w = d.SurfaceWind;
            Assert.AreEqual(240, w.MeanDirection.ActualValue);
            Assert.AreEqual(4, w.MeanSpeed.ActualValue);
            Assert.AreEqual(9, w.SpeedVariations.ActualValue);
            Assert.AreEqual(Unit.MeterPerSecond, w.MeanSpeed.ActualUnit);
            var v = d.Visibility;
            Assert.AreEqual(2500, v.PrevailingVisibility.ActualValue);
            Assert.AreEqual(1000, v.MinimumVisibility.ActualValue);
            Assert.AreEqual("NW", v.MinimumVisibilityDirection);
            var rs = d.RunwaysVisualRange;
            var r1 = rs[0];
            Assert.AreEqual("32", r1.Runway);
            Assert.AreEqual(400, r1.VisualRange.ActualValue);
            Assert.AreEqual(Tendency.NONE, r1.PastTendency);
            var r2 = rs[1];
            Assert.AreEqual("08C", r2.Runway);
            Assert.AreEqual(4, r2.VisualRange.ActualValue);
            Assert.AreEqual(Tendency.D, r2.PastTendency);
            var pw = d.PresentWeather;
            Assert.AreEqual(2, pw.Count);
            var pw1 = pw[0];
            Assert.AreEqual("+", pw1.IntensityProximity);
            Assert.AreEqual("FZ", pw1.Characteristics);
            Assert.AreEqual(new ReadOnlyCollection<string>(new List<string>() { "RA" }), pw1.Types);
            var pw2 = pw[1];
            Assert.AreEqual("VC", pw2.IntensityProximity);
            Assert.AreEqual(string.Empty, pw2.Characteristics);
            Assert.AreEqual(new ReadOnlyCollection<string>(new List<string>() { "SN" }), pw2.Types);
            var cs = d.Clouds;
            var c = cs[0];
            Assert.AreEqual(CloudAmount.FEW, c.Amount);
            Assert.AreEqual(1500, c.BaseHeight.ActualValue);
            Assert.AreEqual(Unit.Feet, c.BaseHeight.ActualUnit);
            Assert.AreEqual(17, d.AirTemperature.ActualValue);
            Assert.AreEqual(10, d.DewPointTemperature.ActualValue);
            Assert.AreEqual(1009, d.Pressure.ActualValue);
            Assert.AreEqual(Unit.HectoPascal, d.Pressure.ActualUnit);
            var rw = d.RecentWeather;
            Assert.AreEqual("FZ", rw.Characteristics);
            Assert.AreEqual("RA", rw.Types[0]);
            Assert.AreEqual(new string[] { "03" }, d.WindshearRunways);
            Assert.IsFalse(d.WindshearAllRunways);
        }

        /// <summary>
        /// Test parsing of a short, valid METAR.
        /// </summary>
        [Test]
        public void TestParseShort()
        {
            // launch decoding
            var d = decoder.ParseStrict("METAR LFPB 190730Z AUTO 17005KT 6000 OVC024 02/00 Q1032 ");
            // compare results
            Assert.IsTrue(d.IsValid);
            Assert.AreEqual(MetarType.METAR, d.Type);
            Assert.AreEqual("LFPB", d.ICAO);
            Assert.AreEqual(19, d.Day);
            Assert.AreEqual("07:30 UTC", d.Time);
            Assert.AreEqual("AUTO", d.Status);
            var w = d.SurfaceWind;
            Assert.AreEqual(170, w.MeanDirection.ActualValue);
            Assert.AreEqual(5, w.MeanSpeed.ActualValue);
            Assert.AreEqual(Unit.Knot, w.MeanSpeed.ActualUnit);
            var v = d.Visibility;
            Assert.AreEqual(6000, v.PrevailingVisibility.ActualValue);
            var cs = d.Clouds;
            var c = cs[0];
            Assert.AreEqual(CloudAmount.OVC, c.Amount);
            Assert.AreEqual(2400, c.BaseHeight.ActualValue);
            Assert.AreEqual(2, d.AirTemperature.ActualValue);
            Assert.AreEqual(0, d.DewPointTemperature.ActualValue);
            Assert.AreEqual(1032, d.Pressure.ActualValue);
            Assert.AreEqual(Unit.HectoPascal, d.Pressure.ActualUnit);
        }

        /// <summary>
        /// Test parsing of a short, invalid METAR, without strict option activated.
        /// </summary>
        [Test]
        public void TestParseInvalid()
        {
            // launch decoding
            var d = decoder.ParseNotStrict("METAR LFPB 190730Z AUTOPP 17005KT 6000 OVC024 02/00 Q10032 ");
            //                                                 here ^                              ^ and here
            // compare results
            Assert.IsFalse(d.IsValid);
            Assert.AreEqual(2, d.DecodingExceptions.Count);
            Assert.AreEqual(MetarType.METAR, d.Type);
            Assert.AreEqual("LFPB", d.ICAO);
            Assert.AreEqual(19, d.Day);
            Assert.AreEqual("07:30 UTC", d.Time);
            Assert.AreEqual(string.Empty, d.Status);
            var w = d.SurfaceWind;
            Assert.AreEqual(170, w.MeanDirection.ActualValue);
            Assert.AreEqual(5, w.MeanSpeed.ActualValue);
            Assert.AreEqual(Unit.Knot, w.MeanSpeed.ActualUnit);
            var v = d.Visibility;
            Assert.AreEqual(6000, v.PrevailingVisibility.ActualValue);
            var cs = d.Clouds;
            var c = cs[0];
            Assert.AreEqual(CloudAmount.OVC, c.Amount);
            Assert.AreEqual(2400, c.BaseHeight.ActualValue);
            Assert.AreEqual(2, d.AirTemperature.ActualValue);
            Assert.AreEqual(0, d.DewPointTemperature.ActualValue);
            Assert.IsNull(d.Pressure);
        }

        /// <summary>
        /// Test parsing of an invalid METAR, where parsing can continue normally without strict option activated.
        /// </summary>
        [Test]
        public void TestParseInvalidPart()
        {
            // launch decoding
            var d = decoder.ParseNotStrict("METAR LFPB 190730Z AUTOP X17005KT 6000 OVC024 02/00 Q1032 ");
            //                                                here ^ ^ and here
            // compare results
            Assert.IsFalse(d.IsValid);
            // 3 errors because visibility decoder will choke once before finding the right piece of metar
            Assert.AreEqual(3, d.DecodingExceptions.Count);
            Assert.AreEqual(MetarType.METAR, d.Type);
            Assert.AreEqual("LFPB", d.ICAO);
            Assert.AreEqual(19, d.Day);
            Assert.AreEqual("07:30 UTC", d.Time);
            Assert.AreEqual(string.Empty, d.Status);
            Assert.IsNull(d.SurfaceWind);
            var v = d.Visibility;
            Assert.AreEqual(6000, v.PrevailingVisibility.ActualValue);
            var cs = d.Clouds;
            var c = cs[0];
            Assert.AreEqual(CloudAmount.OVC, c.Amount);
            Assert.AreEqual(2400, c.BaseHeight.ActualValue);
            Assert.AreEqual(2, d.AirTemperature.ActualValue);
            Assert.AreEqual(0, d.DewPointTemperature.ActualValue);
            Assert.AreEqual(1032, d.Pressure.ActualValue);
        }

        [Test]
        public void TestParseNoClouds()
        {
            var metar = "PAWI 140753Z AUTO 08034G41KT 1/4SM SN FZFG M18/M21 A2951 RMK PK WND 08041/0752 SLP995 P0000 T11831206 TSNO  VIA AUTODIAL";
            var d = decoder.ParseStrict(metar);
            Assert.IsFalse(d.IsValid);
            d = decoder.ParseNotStrict(metar);
            Assert.IsFalse(d.IsValid);
            Assert.AreEqual(1, d.DecodingExceptions.Count);
            var errors = d.DecodingExceptions;
            var error = errors[0];
            Assert.AreEqual("CloudChunkDecoder", error.ChunkDecoder.GetType().Name);
            Assert.AreEqual(0, d.Clouds.Count);
        }

        /// <summary>
        /// Test parsing of an empty METAR, which is valid.
        /// </summary>
        [Test]
        public void TestParseNil()
        {
            var d = decoder.ParseStrict("METAR LFPO 231027Z NIL");
            Assert.AreEqual("NIL", d.Status);
        }

        /// <summary>
        /// Test parsing of METAR with trailing end-of-message.
        /// </summary>
        [Test]
        public void TestParseEOM()
        {
            var d = decoder.ParseStrict("METAR LFPB 190730Z AUTO 17005KT 6000 OVC024 02/00 Q1032=");
            Assert.IsTrue(d.IsValid);
            Assert.AreEqual("METAR LFPB 190730Z AUTO 17005KT 6000 OVC024 02/00 Q1032", d.RawMetar);
        }

        /// <summary>
        /// Test parsing of a METAR with CAVOK.
        /// </summary>
        [Test]
        public void testParseCAVOK()
        {
            var d = decoder.ParseStrict("METAR LFPO 231027Z AUTO 24004KT CAVOK 02/M08 Q0995");
            Assert.IsTrue(d.Cavok);
            Assert.IsNull(d.Visibility);
            Assert.AreEqual(0, d.Clouds.Count);
            // check that we went to the end of the decoding though
            Assert.AreEqual(995, d.Pressure.ActualValue);
        }

        /// <summary>
        /// Test parsing of invalid METARs
        /// TODO improve this now that strict option exists.
        /// </summary>
        /// [Test]
        [Test, TestCaseSource("ErrorDataset")]
        public void TestParseErrors(Tuple<string, string, string> metar_error)
        {
            // launch decoding
            var d = decoder.ParseNotStrict(metar_error.Item1);
            // check the error triggered
            Assert.IsFalse(d.IsValid);
            var errors = d.DecodingExceptions;
            var first_error = errors[0];
            Assert.AreEqual(metar_error.Item2, first_error.ChunkDecoder.GetType().Name);
            Assert.AreEqual(metar_error.Item3, first_error.RemainingMetar);
        }

        public static List<Tuple<string, string, string>> ErrorDataset()
        {
            return new List<Tuple<string, string, string>>()
            {
                new Tuple<string, string, string>("LFPG aaa bbb cccc", "DatetimeChunkDecoder", "AAA BBB CCCC "),
                new Tuple<string, string, string>("METAR LFPO 231027Z NIL 1234", "ReportStatusChunkDecoder", "NIL 1234 "),
                new Tuple<string, string, string>("METAR LFPO 231027Z AUTO 24004G09MPS 2500 1000NW R32/0400 R08C/0004D FZRAA FEW015", "CloudChunkDecoder", "FZRAA FEW015 "),
            };
        }

        /// <summary>
        /// Test object-wide strict option.
        /// </summary>
        [Test]
        public void TestParseDefaultStrictMode()
        {
            // strict mode, max 1 error triggered
            decoder.SetStrictParsing(true);
            var d = decoder.Parse("LFPG aaa bbb cccc");
            Assert.AreEqual(1, d.DecodingExceptions.Count);
            // not strict: several errors triggered
            decoder.SetStrictParsing(false);
            d = decoder.Parse("LFPG aaa bbb cccc");
            Assert.AreEqual(5, d.DecodingExceptions.Count);
        }

        /// <summary>
        /// Test error reset
        /// </summary>
        [Test]
        public void TestErrorReset()
        {
            var d = decoder.Parse("LFPG aaa bbb cccc");
            Assert.AreEqual(5, d.DecodingExceptions.Count);
            d.ResetDecodingExceptions();
            Assert.AreEqual(0, d.DecodingExceptions.Count);
        }
    }
}
