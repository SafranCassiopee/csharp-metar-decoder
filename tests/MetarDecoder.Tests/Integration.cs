using MetarDecoder.Entity;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static MetarDecoder.Entity.CloudLayer;
using static MetarDecoder.Entity.DecodedMetar;

namespace MetarDecoder.Tests
{
    [TestFixture, Category("Integration")]
    public class Integration
    {
        public readonly ReadOnlyCollection<string> TestMetarSource = new ReadOnlyCollection<string>(new List<string>() {
            "CYFB 271515Z 32017KT 3SM DRSN BKN040 M29/M34 A2957 RMK SC7 SLP019",
            "EETU 271450Z 05005KT 9000 OVC006 01/M00 Q1019",
            "SBGU 271400Z 27006KT 8000 TS VCSH BKN020 FEW030CB BKN080 25/20 Q1017",
            "SBGU 321400Z 27006KT 8000 TS VCSH BKN020 FEW030CB BKN080 25/20"  // this one has errors, on purpose !
        });

        List<DecodedMetar> DecodedMetarsNotStrict;
        List<DecodedMetar> DecodedMetarsStrict;

        [SetUp]
        public void Setup()
        {
            DecodedMetarsNotStrict = TestMetarSource.Select(metar =>
                Decoder.ParseWithMode(metar, false)
            ).ToList();
            DecodedMetarsStrict = TestMetarSource.Select(metar =>
                Decoder.ParseWithMode(metar, true)
            ).ToList();
        }

        [Test]
        public void CYFB271515ZNotStrictTest()
        {
            var decodedMetar = DecodedMetarsNotStrict[0];
            Assert.AreEqual(TestMetarSource[0], decodedMetar.RawMetar);
            Assert.AreEqual(0, decodedMetar.DecodingExceptions.Count);
            Assert.AreEqual(MetarType.NULL, decodedMetar.Type);
            Assert.AreEqual("CYFB", decodedMetar.ICAO);
            Assert.AreEqual(27, decodedMetar.Day);
            Assert.AreEqual("15:15 UTC", decodedMetar.Time);
            Assert.AreEqual(string.Empty, decodedMetar.Status);

            #region SurfaceWind
            Assert.IsNotNull(decodedMetar.SurfaceWind);
            Assert.AreEqual(320, decodedMetar.SurfaceWind.MeanDirection.ActualValue);
            Assert.AreEqual(Value.Unit.Degree, decodedMetar.SurfaceWind.MeanDirection.ActualUnit);
            Assert.IsFalse(decodedMetar.SurfaceWind.VariableDirection);
            Assert.AreEqual(17, decodedMetar.SurfaceWind.MeanSpeed.ActualValue);
            Assert.AreEqual(Value.Unit.Knot, decodedMetar.SurfaceWind.MeanSpeed.ActualUnit);
            Assert.IsNull(decodedMetar.SurfaceWind.SpeedVariations);
            Assert.IsNull(decodedMetar.SurfaceWind.DirectionVariations);
            #endregion

            #region Visibility
            Assert.IsNotNull(decodedMetar.Visibility);
            Assert.AreEqual(3, decodedMetar.Visibility.PrevailingVisibility.ActualValue);
            Assert.AreEqual(Value.Unit.StatuteMile, decodedMetar.Visibility.PrevailingVisibility.ActualUnit);
            Assert.IsNull(decodedMetar.Visibility.MinimumVisibility);
            Assert.IsNull(decodedMetar.Visibility.MinimumVisibilityDirection);
            Assert.IsFalse(decodedMetar.Visibility.NDV);
            #endregion

            Assert.IsFalse(decodedMetar.Cavok);
            Assert.IsNotNull(decodedMetar.RunwaysVisualRange);
            Assert.AreEqual(0, decodedMetar.RunwaysVisualRange.Count);

            #region PresentWeather
            Assert.AreEqual(1, decodedMetar.PresentWeather.Count);
            Assert.AreEqual(string.Empty, decodedMetar.PresentWeather[0].IntensityProximity);
            Assert.AreEqual("DR", decodedMetar.PresentWeather[0].Characteristics);
            Assert.AreEqual(1, decodedMetar.PresentWeather[0].Types.Count);
            Assert.AreEqual("SN", decodedMetar.PresentWeather[0].Types[0]);
            #endregion

            #region Clouds
            Assert.AreEqual(1, decodedMetar.Clouds.Count);
            Assert.AreEqual(CloudAmount.BKN, decodedMetar.Clouds[0].Amount);
            Assert.AreEqual(4000, decodedMetar.Clouds[0].BaseHeight.ActualValue);
            Assert.AreEqual(Value.Unit.Feet, decodedMetar.Clouds[0].BaseHeight.ActualUnit);
            Assert.AreEqual(CloudType.NULL, decodedMetar.Clouds[0].Type);
            #endregion

            #region Temperatures & pressure
            Assert.AreEqual(-29, decodedMetar.AirTemperature.ActualValue);
            Assert.AreEqual(Value.Unit.DegreeCelsius, decodedMetar.AirTemperature.ActualUnit);

            Assert.AreEqual(-34, decodedMetar.DewPointTemperature.ActualValue);
            Assert.AreEqual(Value.Unit.DegreeCelsius, decodedMetar.DewPointTemperature.ActualUnit);

            Assert.AreEqual(29.57, decodedMetar.Pressure.ActualValue);
            Assert.AreEqual(Value.Unit.MercuryInch, decodedMetar.Pressure.ActualUnit);
            #endregion

            Assert.IsNull(decodedMetar.RecentWeather);
            Assert.IsNull(decodedMetar.WindshearAllRunways);
            Assert.AreEqual(0, decodedMetar.WindshearRunways.Count);
        }

        [Test]
        public void EETU271450ZNotStrictTest()
        {
            var decodedMetar = DecodedMetarsNotStrict[1];
            Assert.AreEqual(TestMetarSource[1], decodedMetar.RawMetar);
            Assert.AreEqual(0, decodedMetar.DecodingExceptions.Count);
            Assert.AreEqual(MetarType.NULL, decodedMetar.Type);
            Assert.AreEqual("EETU", decodedMetar.ICAO);
            Assert.AreEqual(27, decodedMetar.Day);
            Assert.AreEqual("14:50 UTC", decodedMetar.Time);
            Assert.AreEqual(string.Empty, decodedMetar.Status);

            #region SurfaceWind
            Assert.IsNotNull(decodedMetar.SurfaceWind);
            Assert.AreEqual(50, decodedMetar.SurfaceWind.MeanDirection.ActualValue);
            Assert.AreEqual(Value.Unit.Degree, decodedMetar.SurfaceWind.MeanDirection.ActualUnit);
            Assert.IsFalse(decodedMetar.SurfaceWind.VariableDirection);
            Assert.AreEqual(5, decodedMetar.SurfaceWind.MeanSpeed.ActualValue);
            Assert.AreEqual(Value.Unit.Knot, decodedMetar.SurfaceWind.MeanSpeed.ActualUnit);
            Assert.IsNull(decodedMetar.SurfaceWind.SpeedVariations);
            Assert.IsNull(decodedMetar.SurfaceWind.DirectionVariations);
            #endregion

            #region Visibility
            Assert.IsNotNull(decodedMetar.Visibility);
            Assert.AreEqual(9000, decodedMetar.Visibility.PrevailingVisibility.ActualValue);
            Assert.AreEqual(Value.Unit.Meter, decodedMetar.Visibility.PrevailingVisibility.ActualUnit);
            Assert.IsNull(decodedMetar.Visibility.MinimumVisibility);
            Assert.IsNull(decodedMetar.Visibility.MinimumVisibilityDirection);
            Assert.IsFalse(decodedMetar.Visibility.NDV);
            #endregion

            Assert.IsFalse(decodedMetar.Cavok);
            Assert.IsNotNull(decodedMetar.RunwaysVisualRange);
            Assert.AreEqual(0, decodedMetar.RunwaysVisualRange.Count);

            #region PresentWeather
            Assert.AreEqual(0, decodedMetar.PresentWeather.Count);
            #endregion

            #region Clouds
            Assert.AreEqual(1, decodedMetar.Clouds.Count);
            Assert.AreEqual(CloudAmount.OVC, decodedMetar.Clouds[0].Amount);
            Assert.AreEqual(600, decodedMetar.Clouds[0].BaseHeight.ActualValue);
            Assert.AreEqual(Value.Unit.Feet, decodedMetar.Clouds[0].BaseHeight.ActualUnit);
            Assert.AreEqual(CloudType.NULL, decodedMetar.Clouds[0].Type);
            #endregion

            #region Temperatures & pressure
            Assert.AreEqual(1, decodedMetar.AirTemperature.ActualValue);
            Assert.AreEqual(Value.Unit.DegreeCelsius, decodedMetar.AirTemperature.ActualUnit);

            Assert.AreEqual(0, decodedMetar.DewPointTemperature.ActualValue);
            Assert.AreEqual(Value.Unit.DegreeCelsius, decodedMetar.DewPointTemperature.ActualUnit);

            Assert.AreEqual(1019, decodedMetar.Pressure.ActualValue);
            Assert.AreEqual(Value.Unit.HectoPascal, decodedMetar.Pressure.ActualUnit);
            #endregion

            Assert.IsNull(decodedMetar.RecentWeather);
            Assert.IsNull(decodedMetar.WindshearAllRunways);
            Assert.AreEqual(0, decodedMetar.WindshearRunways.Count);
        }

        [Test]
        public void SBGU271400ZNotStrictTest()
        {
            var decodedMetar = DecodedMetarsNotStrict[2];
            Assert.AreEqual(TestMetarSource[2], decodedMetar.RawMetar);
            Assert.AreEqual(0, decodedMetar.DecodingExceptions.Count);
            Assert.AreEqual(MetarType.NULL, decodedMetar.Type);
            Assert.AreEqual("SBGU", decodedMetar.ICAO);
            Assert.AreEqual(27, decodedMetar.Day);
            Assert.AreEqual("14:00 UTC", decodedMetar.Time);
            Assert.AreEqual(string.Empty, decodedMetar.Status);

            #region SurfaceWind
            Assert.IsNotNull(decodedMetar.SurfaceWind);
            Assert.AreEqual(270, decodedMetar.SurfaceWind.MeanDirection.ActualValue);
            Assert.AreEqual(Value.Unit.Degree, decodedMetar.SurfaceWind.MeanDirection.ActualUnit);
            Assert.IsFalse(decodedMetar.SurfaceWind.VariableDirection);
            Assert.AreEqual(6, decodedMetar.SurfaceWind.MeanSpeed.ActualValue);
            Assert.AreEqual(Value.Unit.Knot, decodedMetar.SurfaceWind.MeanSpeed.ActualUnit);
            Assert.IsNull(decodedMetar.SurfaceWind.SpeedVariations);
            Assert.IsNull(decodedMetar.SurfaceWind.DirectionVariations);
            #endregion

            #region Visibility
            Assert.IsNotNull(decodedMetar.Visibility);
            Assert.AreEqual(8000, decodedMetar.Visibility.PrevailingVisibility.ActualValue);
            Assert.AreEqual(Value.Unit.Meter, decodedMetar.Visibility.PrevailingVisibility.ActualUnit);
            Assert.IsNull(decodedMetar.Visibility.MinimumVisibility);
            Assert.IsNull(decodedMetar.Visibility.MinimumVisibilityDirection);
            Assert.IsFalse(decodedMetar.Visibility.NDV);
            #endregion

            Assert.IsFalse(decodedMetar.Cavok);
            Assert.IsNotNull(decodedMetar.RunwaysVisualRange);
            Assert.AreEqual(0, decodedMetar.RunwaysVisualRange.Count);

            #region PresentWeather
            Assert.AreEqual(2, decodedMetar.PresentWeather.Count);

            Assert.AreEqual(string.Empty, decodedMetar.PresentWeather[0].IntensityProximity);
            Assert.AreEqual("TS", decodedMetar.PresentWeather[0].Characteristics);
            Assert.AreEqual(0, decodedMetar.PresentWeather[0].Types.Count);

            Assert.AreEqual("VC", decodedMetar.PresentWeather[1].IntensityProximity);
            Assert.AreEqual("SH", decodedMetar.PresentWeather[1].Characteristics);
            Assert.AreEqual(0, decodedMetar.PresentWeather[1].Types.Count);
            #endregion

            #region Clouds
            Assert.AreEqual(3, decodedMetar.Clouds.Count);

            Assert.AreEqual(CloudAmount.BKN, decodedMetar.Clouds[0].Amount);
            Assert.AreEqual(2000, decodedMetar.Clouds[0].BaseHeight.ActualValue);
            Assert.AreEqual(Value.Unit.Feet, decodedMetar.Clouds[0].BaseHeight.ActualUnit);
            Assert.AreEqual(CloudType.NULL, decodedMetar.Clouds[0].Type);

            Assert.AreEqual(CloudAmount.FEW, decodedMetar.Clouds[1].Amount);
            Assert.AreEqual(3000, decodedMetar.Clouds[1].BaseHeight.ActualValue);
            Assert.AreEqual(Value.Unit.Feet, decodedMetar.Clouds[1].BaseHeight.ActualUnit);
            Assert.AreEqual(CloudType.CB, decodedMetar.Clouds[1].Type);

            Assert.AreEqual(CloudAmount.BKN, decodedMetar.Clouds[2].Amount);
            Assert.AreEqual(8000, decodedMetar.Clouds[2].BaseHeight.ActualValue);
            Assert.AreEqual(Value.Unit.Feet, decodedMetar.Clouds[2].BaseHeight.ActualUnit);
            Assert.AreEqual(CloudType.NULL, decodedMetar.Clouds[2].Type);
            #endregion

            #region Temperatures & pressure
            Assert.AreEqual(25, decodedMetar.AirTemperature.ActualValue);
            Assert.AreEqual(Value.Unit.DegreeCelsius, decodedMetar.AirTemperature.ActualUnit);

            Assert.AreEqual(20, decodedMetar.DewPointTemperature.ActualValue);
            Assert.AreEqual(Value.Unit.DegreeCelsius, decodedMetar.DewPointTemperature.ActualUnit);

            Assert.AreEqual(1017, decodedMetar.Pressure.ActualValue);
            Assert.AreEqual(Value.Unit.HectoPascal, decodedMetar.Pressure.ActualUnit);
            #endregion

            Assert.IsNull(decodedMetar.RecentWeather);
            Assert.IsNull(decodedMetar.WindshearAllRunways);
            Assert.AreEqual(0, decodedMetar.WindshearRunways.Count);
        }

        [Test]
        public void SBGU321400ZNotStrictTest()
        {
            var decodedMetar = DecodedMetarsNotStrict[3];
            Assert.AreEqual(TestMetarSource[3], decodedMetar.RawMetar);
            //PHP version says there are 0, and reports 2 in the "Invalid format" message at top of page
            //issues probably lies with strict/no strict error handling

            Assert.AreEqual(3, decodedMetar.DecodingExceptions.Count);
            Assert.AreEqual(MetarType.NULL, decodedMetar.Type);
            Assert.AreEqual("SBGU", decodedMetar.ICAO);
            Assert.IsNull(decodedMetar.Day);
            Assert.AreEqual(string.Empty, decodedMetar.Time);
            Assert.AreEqual(string.Empty, decodedMetar.Status);

            #region SurfaceWind
            Assert.IsNotNull(decodedMetar.SurfaceWind);
            //needs fixing
            Assert.AreEqual(270, decodedMetar.SurfaceWind.MeanDirection.ActualValue);
            //needs fixing
            Assert.AreEqual(Value.Unit.Degree, decodedMetar.SurfaceWind.MeanDirection.ActualUnit);
            Assert.IsFalse(decodedMetar.SurfaceWind.VariableDirection);
            //needs fixing
            Assert.AreEqual(6, decodedMetar.SurfaceWind.MeanSpeed.ActualValue);
            //needs fixing
            Assert.AreEqual(Value.Unit.Knot, decodedMetar.SurfaceWind.MeanSpeed.ActualUnit);
            Assert.IsNull(decodedMetar.SurfaceWind.SpeedVariations);
            Assert.IsNull(decodedMetar.SurfaceWind.DirectionVariations);
            #endregion

            #region Visibility
            Assert.IsNotNull(decodedMetar.Visibility);
            //needs fixing
            Assert.AreEqual(8000, decodedMetar.Visibility.PrevailingVisibility.ActualValue);
            //needs fixing
            Assert.AreEqual(Value.Unit.Meter, decodedMetar.Visibility.PrevailingVisibility.ActualUnit);
            Assert.IsNull(decodedMetar.Visibility.MinimumVisibility);
            Assert.IsNull(decodedMetar.Visibility.MinimumVisibilityDirection);
            Assert.IsFalse(decodedMetar.Visibility.NDV);
            #endregion

            Assert.IsFalse(decodedMetar.Cavok);
            Assert.IsNotNull(decodedMetar.RunwaysVisualRange);
            Assert.AreEqual(0, decodedMetar.RunwaysVisualRange.Count);

            #region PresentWeather
            //needs fixing
            Assert.AreEqual(2, decodedMetar.PresentWeather.Count);

            Assert.AreEqual(string.Empty, decodedMetar.PresentWeather[0].IntensityProximity);
            Assert.AreEqual("TS", decodedMetar.PresentWeather[0].Characteristics);
            Assert.AreEqual(0, decodedMetar.PresentWeather[0].Types.Count);

            Assert.AreEqual("VC", decodedMetar.PresentWeather[1].IntensityProximity);
            Assert.AreEqual("SH", decodedMetar.PresentWeather[1].Characteristics);
            Assert.AreEqual(0, decodedMetar.PresentWeather[1].Types.Count);
            #endregion

            #region Clouds
            //needs fixing
            Assert.AreEqual(3, decodedMetar.Clouds.Count);

            Assert.AreEqual(CloudAmount.BKN, decodedMetar.Clouds[0].Amount);
            Assert.AreEqual(2000, decodedMetar.Clouds[0].BaseHeight.ActualValue);
            Assert.AreEqual(Value.Unit.Feet, decodedMetar.Clouds[0].BaseHeight.ActualUnit);
            Assert.AreEqual(CloudType.NULL, decodedMetar.Clouds[0].Type);

            Assert.AreEqual(CloudAmount.FEW, decodedMetar.Clouds[1].Amount);
            Assert.AreEqual(3000, decodedMetar.Clouds[1].BaseHeight.ActualValue);
            Assert.AreEqual(Value.Unit.Feet, decodedMetar.Clouds[1].BaseHeight.ActualUnit);
            Assert.AreEqual(CloudType.CB, decodedMetar.Clouds[1].Type);

            Assert.AreEqual(CloudAmount.BKN, decodedMetar.Clouds[2].Amount);
            Assert.AreEqual(8000, decodedMetar.Clouds[2].BaseHeight.ActualValue);
            Assert.AreEqual(Value.Unit.Feet, decodedMetar.Clouds[2].BaseHeight.ActualUnit);
            Assert.AreEqual(CloudType.NULL, decodedMetar.Clouds[2].Type);
            #endregion

            #region Temperatures & pressure
            //needs fixing
            Assert.AreEqual(25, decodedMetar.AirTemperature.ActualValue);
            Assert.AreEqual(Value.Unit.DegreeCelsius, decodedMetar.AirTemperature.ActualUnit);

            Assert.AreEqual(20, decodedMetar.DewPointTemperature.ActualValue);
            Assert.AreEqual(Value.Unit.DegreeCelsius, decodedMetar.DewPointTemperature.ActualUnit);

            Assert.IsNull(decodedMetar.Pressure);
            #endregion

            Assert.IsNull(decodedMetar.RecentWeather);
            Assert.IsNull(decodedMetar.WindshearAllRunways);
            Assert.AreEqual(0, decodedMetar.WindshearRunways.Count);
        }

        [Test]
        public void CYFB271515ZStrictTest()
        {
            var decodedMetar = DecodedMetarsStrict[0];
            Assert.AreEqual(TestMetarSource[0], decodedMetar.RawMetar);
            Assert.AreEqual(0, decodedMetar.DecodingExceptions.Count);
            Assert.AreEqual(MetarType.NULL, decodedMetar.Type);
            Assert.AreEqual("CYFB", decodedMetar.ICAO);
            Assert.AreEqual(27, decodedMetar.Day);
            Assert.AreEqual("15:15 UTC", decodedMetar.Time);
            Assert.AreEqual(string.Empty, decodedMetar.Status);

            #region SurfaceWind
            Assert.IsNotNull(decodedMetar.SurfaceWind);
            Assert.AreEqual(320, decodedMetar.SurfaceWind.MeanDirection.ActualValue);
            Assert.AreEqual(Value.Unit.Degree, decodedMetar.SurfaceWind.MeanDirection.ActualUnit);
            Assert.IsFalse(decodedMetar.SurfaceWind.VariableDirection);
            Assert.AreEqual(17, decodedMetar.SurfaceWind.MeanSpeed.ActualValue);
            Assert.AreEqual(Value.Unit.Knot, decodedMetar.SurfaceWind.MeanSpeed.ActualUnit);
            Assert.IsNull(decodedMetar.SurfaceWind.SpeedVariations);
            Assert.IsNull(decodedMetar.SurfaceWind.DirectionVariations);
            #endregion

            #region Visibility
            Assert.IsNotNull(decodedMetar.Visibility);
            Assert.AreEqual(3, decodedMetar.Visibility.PrevailingVisibility.ActualValue);
            Assert.AreEqual(Value.Unit.StatuteMile, decodedMetar.Visibility.PrevailingVisibility.ActualUnit);
            Assert.IsNull(decodedMetar.Visibility.MinimumVisibility);
            Assert.IsNull(decodedMetar.Visibility.MinimumVisibilityDirection);
            Assert.IsFalse(decodedMetar.Visibility.NDV);
            #endregion

            Assert.IsFalse(decodedMetar.Cavok);
            Assert.IsNotNull(decodedMetar.RunwaysVisualRange);
            Assert.AreEqual(0, decodedMetar.RunwaysVisualRange.Count);

            #region PresentWeather
            Assert.AreEqual(1, decodedMetar.PresentWeather.Count);
            Assert.AreEqual(string.Empty, decodedMetar.PresentWeather[0].IntensityProximity);
            Assert.AreEqual("DR", decodedMetar.PresentWeather[0].Characteristics);
            Assert.AreEqual(1, decodedMetar.PresentWeather[0].Types.Count);
            Assert.AreEqual("SN", decodedMetar.PresentWeather[0].Types[0]);
            #endregion

            #region Clouds
            Assert.AreEqual(1, decodedMetar.Clouds.Count);
            Assert.AreEqual(CloudAmount.BKN, decodedMetar.Clouds[0].Amount);
            Assert.AreEqual(4000, decodedMetar.Clouds[0].BaseHeight.ActualValue);
            Assert.AreEqual(Value.Unit.Feet, decodedMetar.Clouds[0].BaseHeight.ActualUnit);
            Assert.AreEqual(CloudType.NULL, decodedMetar.Clouds[0].Type);
            #endregion

            #region Temperatures & pressure
            Assert.AreEqual(-29, decodedMetar.AirTemperature.ActualValue);
            Assert.AreEqual(Value.Unit.DegreeCelsius, decodedMetar.AirTemperature.ActualUnit);

            Assert.AreEqual(-34, decodedMetar.DewPointTemperature.ActualValue);
            Assert.AreEqual(Value.Unit.DegreeCelsius, decodedMetar.DewPointTemperature.ActualUnit);

            Assert.AreEqual(29.57, decodedMetar.Pressure.ActualValue);
            Assert.AreEqual(Value.Unit.MercuryInch, decodedMetar.Pressure.ActualUnit);
            #endregion

            Assert.IsNull(decodedMetar.RecentWeather);
            Assert.IsNull(decodedMetar.WindshearAllRunways);
            Assert.AreEqual(0, decodedMetar.WindshearRunways.Count);
        }

        [Test]
        public void EETU271450ZStrictTest()
        {
            var decodedMetar = DecodedMetarsStrict[1];
            Assert.AreEqual(TestMetarSource[1], decodedMetar.RawMetar);
            Assert.AreEqual(0, decodedMetar.DecodingExceptions.Count);
            Assert.AreEqual(MetarType.NULL, decodedMetar.Type);
            Assert.AreEqual("EETU", decodedMetar.ICAO);
            Assert.AreEqual(27, decodedMetar.Day);
            Assert.AreEqual("14:50 UTC", decodedMetar.Time);
            Assert.AreEqual(string.Empty, decodedMetar.Status);

            #region SurfaceWind
            Assert.IsNotNull(decodedMetar.SurfaceWind);
            Assert.AreEqual(50, decodedMetar.SurfaceWind.MeanDirection.ActualValue);
            Assert.AreEqual(Value.Unit.Degree, decodedMetar.SurfaceWind.MeanDirection.ActualUnit);
            Assert.IsFalse(decodedMetar.SurfaceWind.VariableDirection);
            Assert.AreEqual(5, decodedMetar.SurfaceWind.MeanSpeed.ActualValue);
            Assert.AreEqual(Value.Unit.Knot, decodedMetar.SurfaceWind.MeanSpeed.ActualUnit);
            Assert.IsNull(decodedMetar.SurfaceWind.SpeedVariations);
            Assert.IsNull(decodedMetar.SurfaceWind.DirectionVariations);
            #endregion

            #region Visibility
            Assert.IsNotNull(decodedMetar.Visibility);
            Assert.AreEqual(9000, decodedMetar.Visibility.PrevailingVisibility.ActualValue);
            Assert.AreEqual(Value.Unit.Meter, decodedMetar.Visibility.PrevailingVisibility.ActualUnit);
            Assert.IsNull(decodedMetar.Visibility.MinimumVisibility);
            Assert.IsNull(decodedMetar.Visibility.MinimumVisibilityDirection);
            Assert.IsFalse(decodedMetar.Visibility.NDV);
            #endregion

            Assert.IsFalse(decodedMetar.Cavok);
            Assert.IsNotNull(decodedMetar.RunwaysVisualRange);
            Assert.AreEqual(0, decodedMetar.RunwaysVisualRange.Count);

            #region PresentWeather
            Assert.AreEqual(0, decodedMetar.PresentWeather.Count);
            #endregion

            #region Clouds
            Assert.AreEqual(1, decodedMetar.Clouds.Count);
            Assert.AreEqual(CloudAmount.OVC, decodedMetar.Clouds[0].Amount);
            Assert.AreEqual(600, decodedMetar.Clouds[0].BaseHeight.ActualValue);
            Assert.AreEqual(Value.Unit.Feet, decodedMetar.Clouds[0].BaseHeight.ActualUnit);
            Assert.AreEqual(CloudType.NULL, decodedMetar.Clouds[0].Type);
            #endregion

            #region Temperatures & pressure
            Assert.AreEqual(1, decodedMetar.AirTemperature.ActualValue);
            Assert.AreEqual(Value.Unit.DegreeCelsius, decodedMetar.AirTemperature.ActualUnit);

            Assert.AreEqual(0, decodedMetar.DewPointTemperature.ActualValue);
            Assert.AreEqual(Value.Unit.DegreeCelsius, decodedMetar.DewPointTemperature.ActualUnit);

            Assert.AreEqual(1019, decodedMetar.Pressure.ActualValue);
            Assert.AreEqual(Value.Unit.HectoPascal, decodedMetar.Pressure.ActualUnit);
            #endregion

            Assert.IsNull(decodedMetar.RecentWeather);
            Assert.IsNull(decodedMetar.WindshearAllRunways);
            Assert.AreEqual(0, decodedMetar.WindshearRunways.Count);
        }

        [Test]
        public void SBGU271400ZStrictTest()
        {
            var decodedMetar = DecodedMetarsStrict[2];
            Assert.AreEqual(TestMetarSource[2], decodedMetar.RawMetar);
            Assert.AreEqual(0, decodedMetar.DecodingExceptions.Count);
            Assert.AreEqual(MetarType.NULL, decodedMetar.Type);
            Assert.AreEqual("SBGU", decodedMetar.ICAO);
            Assert.AreEqual(27, decodedMetar.Day);
            Assert.AreEqual("14:00 UTC", decodedMetar.Time);
            Assert.AreEqual(string.Empty, decodedMetar.Status);

            #region SurfaceWind
            Assert.IsNotNull(decodedMetar.SurfaceWind);
            Assert.AreEqual(270, decodedMetar.SurfaceWind.MeanDirection.ActualValue);
            Assert.AreEqual(Value.Unit.Degree, decodedMetar.SurfaceWind.MeanDirection.ActualUnit);
            Assert.IsFalse(decodedMetar.SurfaceWind.VariableDirection);
            Assert.AreEqual(6, decodedMetar.SurfaceWind.MeanSpeed.ActualValue);
            Assert.AreEqual(Value.Unit.Knot, decodedMetar.SurfaceWind.MeanSpeed.ActualUnit);
            Assert.IsNull(decodedMetar.SurfaceWind.SpeedVariations);
            Assert.IsNull(decodedMetar.SurfaceWind.DirectionVariations);
            #endregion

            #region Visibility
            Assert.IsNotNull(decodedMetar.Visibility);
            Assert.AreEqual(8000, decodedMetar.Visibility.PrevailingVisibility.ActualValue);
            Assert.AreEqual(Value.Unit.Meter, decodedMetar.Visibility.PrevailingVisibility.ActualUnit);
            Assert.IsNull(decodedMetar.Visibility.MinimumVisibility);
            Assert.IsNull(decodedMetar.Visibility.MinimumVisibilityDirection);
            Assert.IsFalse(decodedMetar.Visibility.NDV);
            #endregion

            Assert.IsFalse(decodedMetar.Cavok);
            Assert.IsNotNull(decodedMetar.RunwaysVisualRange);
            Assert.AreEqual(0, decodedMetar.RunwaysVisualRange.Count);

            #region PresentWeather
            Assert.AreEqual(2, decodedMetar.PresentWeather.Count);

            Assert.AreEqual(string.Empty, decodedMetar.PresentWeather[0].IntensityProximity);
            Assert.AreEqual("TS", decodedMetar.PresentWeather[0].Characteristics);
            Assert.AreEqual(0, decodedMetar.PresentWeather[0].Types.Count);

            Assert.AreEqual("VC", decodedMetar.PresentWeather[1].IntensityProximity);
            Assert.AreEqual("SH", decodedMetar.PresentWeather[1].Characteristics);
            Assert.AreEqual(0, decodedMetar.PresentWeather[1].Types.Count);
            #endregion

            #region Clouds
            Assert.AreEqual(3, decodedMetar.Clouds.Count);

            Assert.AreEqual(CloudAmount.BKN, decodedMetar.Clouds[0].Amount);
            Assert.AreEqual(2000, decodedMetar.Clouds[0].BaseHeight.ActualValue);
            Assert.AreEqual(Value.Unit.Feet, decodedMetar.Clouds[0].BaseHeight.ActualUnit);
            Assert.AreEqual(CloudType.NULL, decodedMetar.Clouds[0].Type);

            Assert.AreEqual(CloudAmount.FEW, decodedMetar.Clouds[1].Amount);
            Assert.AreEqual(3000, decodedMetar.Clouds[1].BaseHeight.ActualValue);
            Assert.AreEqual(Value.Unit.Feet, decodedMetar.Clouds[1].BaseHeight.ActualUnit);
            Assert.AreEqual(CloudType.CB, decodedMetar.Clouds[1].Type);

            Assert.AreEqual(CloudAmount.BKN, decodedMetar.Clouds[2].Amount);
            Assert.AreEqual(8000, decodedMetar.Clouds[2].BaseHeight.ActualValue);
            Assert.AreEqual(Value.Unit.Feet, decodedMetar.Clouds[2].BaseHeight.ActualUnit);
            Assert.AreEqual(CloudType.NULL, decodedMetar.Clouds[2].Type);
            #endregion

            #region Temperatures & pressure
            Assert.AreEqual(25, decodedMetar.AirTemperature.ActualValue);
            Assert.AreEqual(Value.Unit.DegreeCelsius, decodedMetar.AirTemperature.ActualUnit);

            Assert.AreEqual(20, decodedMetar.DewPointTemperature.ActualValue);
            Assert.AreEqual(Value.Unit.DegreeCelsius, decodedMetar.DewPointTemperature.ActualUnit);

            Assert.AreEqual(1017, decodedMetar.Pressure.ActualValue);
            Assert.AreEqual(Value.Unit.HectoPascal, decodedMetar.Pressure.ActualUnit);
            #endregion

            Assert.IsNull(decodedMetar.RecentWeather);
            Assert.IsNull(decodedMetar.WindshearAllRunways);
            Assert.AreEqual(0, decodedMetar.WindshearRunways.Count);
        }

        [Test]
        public void SBGU321400ZStrictTest()
        {
            var decodedMetar = DecodedMetarsStrict[3];
            Assert.AreEqual(TestMetarSource[3], decodedMetar.RawMetar);
            //PHP version says there are 0, and reports 2 in the "Invalid format" message at top of page
            //issues probably lies with strict/no strict error handling

            Assert.AreEqual(1, decodedMetar.DecodingExceptions.Count);
            Assert.AreEqual(MetarType.NULL, decodedMetar.Type);
            Assert.AreEqual("SBGU", decodedMetar.ICAO);
            Assert.IsNull(decodedMetar.Day);
            Assert.AreEqual(string.Empty, decodedMetar.Time);
            Assert.AreEqual(string.Empty, decodedMetar.Status);

            #region SurfaceWind
            Assert.IsNull(decodedMetar.SurfaceWind);
            #endregion

            #region Visibility
            Assert.IsNull(decodedMetar.Visibility);
            #endregion

            Assert.IsFalse(decodedMetar.Cavok);
            Assert.IsNotNull(decodedMetar.RunwaysVisualRange);
            Assert.AreEqual(0, decodedMetar.RunwaysVisualRange.Count);

            #region PresentWeather
            Assert.AreEqual(0, decodedMetar.PresentWeather.Count);
            #endregion

            #region Clouds
            Assert.AreEqual(0, decodedMetar.Clouds.Count);
            #endregion

            #region Temperatures & pressure
            Assert.IsNull(decodedMetar.AirTemperature);
            Assert.IsNull(decodedMetar.DewPointTemperature);
            Assert.IsNull(decodedMetar.Pressure);
            #endregion

            Assert.IsNull(decodedMetar.RecentWeather);
            Assert.IsNull(decodedMetar.WindshearAllRunways);
            Assert.IsNull(decodedMetar.WindshearRunways);
        }
    }
}
