C# METAR decoder
=================

[![License](https://poser.pugx.org/safran-cassiopee/csharp-metar-decoder/license.svg)](https://packagist.org/packages/safran-cassiopee/csharp-metar-decoder)
[![Build Status](https://travis-ci.org/SafranCassiopee/csharp-metar-decoder.svg)](https://travis-ci.org/SafranCassiopee/csharp-metar-decoder)
[![Coverage Status](https://coveralls.io/repos/github/SafranCassiopee/csharp-metar-decoder/badge.svg?branch=master)](https://coveralls.io/github/SafranCassiopee/csharp-metar-decoder?branch=master)
[![Latest Stable Version](https://poser.pugx.org/safran-cassiopee/csharp-metar-decoder/v/stable.svg)](https://packagist.org/packages/safran-cassiopee/csharp-metar-decoder)

A .NET library to decode METAR strings, fully unit tested (100% code coverage)

This is largely based on [SafranCassiopee/php-metar-decoder](https://github.com/SafranCassiopee/php-metar-decoder)

They use csharp-metar-decoder in production:

- [Safran AGS ](https://www.safran-electronics-defense.com/aerospace/commercial-aircraft/information-system/analysis-ground-station-ags) (private)
- Your service here ? Submit a pull request or open an issue !

Introduction
------------

This piece of software is a library package that provides a parser to decode raw METAR observation.

METAR is a format made for weather information reporting. METAR weather reports are predominantly used by pilots and by meteorologists, who use it to assist in weather forecasting.
Raw METAR format is highly standardized through the International Civil Aviation Organization (ICAO).

*    [METAR definition on wikipedia](http://en.wikipedia.org/wiki/METAR)
*    [METAR format specification](http://www.wmo.int/pages/prog/www/WMOCodes/WMO306_vI1/VolumeI.1.html)
*    [METAR documentation](http://meteocentre.com/doc/metar.html)

Requirements
------------

This library package only requires .NET >= 4.5

It is currently tested automatically for .NET >= 4.5 using [nUnit 3.9.0](http://nunit.org/).

Although this is provided as a library project, a command line version (StartMetarDecoder) is also included that can be used as both an example and a starting point.
StartMetarDecoder requires [CommandLineParser](https://github.com/commandlineparser/commandline).

Usage:

```shell
StartMetarDecoder.exe --Metar "LFPO 231027Z AUTO 24004G09MPS 2500 1000NW R32/0400 R08C/0004D +FZRA VCSN //FEW015 17/10 Q1009 REFZRA WS R03"
```

If you want to integrate the library easily in your project, you should consider using the official nuget package available from https://www.nuget.org/.

```
nuget install csharp-metar-decoder
```

It is not mandatory though.

Setup
-----

- With nuget.exe *(recommended)*

From the Package Manager Console in Visual Studio

```shell
nuget install csharp-metar-decoder
```

Add a reference to the library, then add the following using directives:

```csharp
using csharp_metar_decoder;
using csharp_metar_decoder.entity;
```

- By hand

Download the latest release from [github](https://github.com/SafranCassiopee/csharp-metar-decoder/releases)

Extract it wherever you want in your project. The library itself is in the csharp-metar-decoder/ directory, the other directories are not mandatory for the library to work.

Add the csharp-metar-decoder project to your solution, then add a reference to it in your own project. Finally, add the same using directives than above.

Usage
-----

Instantiate the decoder and launch it on a METAR string.
The returned object is a DecodedMetar object from which you can retrieve all the weather properties that have been decoded.

All values who have a unit are based on the `Value` object which provides the ActualValue and ActualUnit properties

Check the [DecodedMetar class](https://github.com/SafranCassiopee/csharp-metar-decoder/blob/master/csharp-metar-decoder/Entity/DecodedMetar.cs) for the structure of the resulting object

```csharp

  var d = MetarDecoder.ParseWithMode("METAR LFPO 231027Z AUTO 24004G09MPS 2500 1000NW R32/0400 R08C/0004D +FZRA VCSN //FEW015 17/10 Q1009 REFZRA WS R03");

  //context information
  d.IsValid; //true
  d.RawMetar; //"METAR LFPO 231027Z AUTO 24004G09MPS 2500 1000NW R32/0400 R08C/0004D +FZRA VCSN //FEW015 17/10 Q1009 REFZRA WS R03"
  d.Type; //MetarType.METAR
  d.Icao; //"LFPO"
  d.Day; //23
  d.Time; //'10:27 UTC"
  d.Status; //"AUTO"

  //surface wind
  var sw = d.SurfaceWind; //SurfaceWind object
  sw.MeanDirection.ActualValue; //240
  sw.MeanSpeed.ActualValue; //4
  sw.SpeedVariations.ActualValue; //9
  sw.MeanSpeed.ActualUnit; //Value.Unit.MeterPerSecond

  //visibility
  var v = d.Visibility; //Visibility object
  v.PrevailingVisibility.ActualValue; //2500
  v.PrevailingVisibility.ActualUnit; //Value.Unit.Meter
  v.MinimumVisibility.ActualValue; //1000
  v.MinimumVisibilityDirection; //"NW"
  v.NDV; //false

  //runway visual range
  var rvr = d.RunwaysVisualRange; //RunwayVisualRange array
  rvr[0].Runway; //"32"
  rvr[0].VisualRange.ActualValue; //400
  rvr[0].PastTendency; //""
  rvr[1].Runway; //"08C"
  rvr[1].VisualRange.ActualValue; //4
  rvr[1].PastTendency; //"D"

  //present weather
  var pw = d.PresentWeather; //WeatherPhenomenon array
  pw[0].IntensityProximity; //"+"
  pw[0].Characteristics; //"FZ"
  pw[0].Types; //{ "RA" }
  pw[1].IntensityProximity; //'VC'
  pw[1].Characteristics; //null
  pw[1].Types; //{ "SN" }

  // clouds
  var cld = d.Clouds; //CloudLayer array
  cld[0].Amount; //CloudAmount.FEW
  cld[0].BaseHeight.ActualValue; //1500
  cld[0].BaseHeight.ActualUnit; //Value.Unit.Feet

  // temperature
  d.AirTemperature.ActualValue; //17
  d.AirTemperature.ActualUnit; //Value.Unit.DegreeCelsius
  d.DewPointTemperature.ActualValue; //10

  // pressure
  d.Pressure.ActualValue; //1009
  d.Pressure.ActualUnit; //Value.Unit.HectoPascal

  // recent weather
  rw = d.RecentWeather;
  rw.Characteristics; //"FZ"
  rw.Types; //{ "RA" }

  // windshears
  d.WindshearRunways; //{ "03" }

```

About Value objects
-------------------

In the example above, it is assumed that all requested parameters are available. 
In the real world, some fields are not mandatory thus it is important to check that the Value object (containing both the value and its unit) is not null before using it.
What you do in case it's null is totally up to you.

Here is an example:

```csharp
  // check that the dew_point is not null and give it a default value if it is
  var dew_point = d.DewPointTemperature;
  if (dew_point == null) 
  {
      dew_point = new Value(999, Value.Unit.DegreeCelsius);
  }

  // dew_point object can now be accessed safely
  dew_point.ActualValue();
  dew_point.ActualUnit();
```

Value objects also contain their unit, that you can access with the `ActualUnit` property. When you access the `ActualValue` property, you'll get the value in this unit. 

If you want to get the value directly in another unit you can call `GetConvertedValue(unit)`. Supported values are speed, distance and pressure.

Here are all available units for conversion:

```csharp
// speed units:
// Value.Unit.MeterPerSecond
// Value.Unit.KilometerPerHour
// Value.Unit.Knot

// distance units:
// Value.Unit.Meter
// Value.Unit.Feet
// Value.Unit.StatuteMile

// pressure units:
// Value.Unit.HectoPascal
// Value.Unit.MercuryInch

// use on-the-fly conversion
var distance_in_sm = visibility.GetConvertedValue(Value.Unit.StatuteMile);
var speed_kph = speed.GetConvertedValue(Value.Unit.KilometerPerHour);
```

About parsing errors
--------------------

When an unexpected format is encountered for a part of the METAR, the parsing error is logged into the DecodedMetar object itself.

All parsing errors for one metar can be accessed through the `DecodingExceptions` property.

By default parsing will continue when a bad format is encountered. 
But the parser also provides a "strict" mode where parsing stops as soon as an error occurs.
The mode can be set globally for a MetarDecoder object, or just once as you can see in this example:

```csharp

var decoder = new MetarDecoder();
decoder.SetStrictParsing(true);

// change global parsing mode to "strict"
decoder.SetStrictParsing(true);

// this parsing will be made with strict mode
decoder.Parse("...");

// but this one will ignore global mode and will be made with not-strict mode anyway
decoder.ParseNotStrict("...");

// change global parsing mode to "not-strict"
decoder.SetStrictParsing(false);

// this parsing will be made with no-strict mode
decoder.Parse("...");

// but this one will ignore global mode and will be made with strict mode anyway
decoder.ParseStrict("...");

```

About parsing errors, again
---------------------------

In non-strict mode, it is possible to get a parsing error for a given chunk decoder, while still getting the decoded information for this chunk in the end. How is that possible ?

It is because non-strict mode not only continues decoding where there is an error, it also tries the parsing again on the "next chunk" (based on whitespace separator). But all errors on first try will remain logged even if the second try suceeded.

Let's say you have this chunk `AAA 12003KPH ...` provided to the SurfaceWind chunk decoder. This decoder will choke on `AAA`, will try to decode `12003KPH` and will succeed. The first exception for surface wind decoder will be kept but the SurfaceWind object will be filled with some information.

All of this does not apply to strict mode as parsing is interrupted on first parsing error in this case.

Contribute
----------

If you find a valid METAR that is badly parsed by this library, please open a github issue with all possible details:

- the full METAR causing problem
- the parsing exception returned by the library
- how you expected the decoder to behave
- anything to support your proposal (links to official websites appreciated)

If you want to improve or enrich the test suite, fork the repository and submit your changes with a pull request.

If you have any other idea to improve the library, please use github issues or directly pull requests depending on what you're more comfortable with.

In order to contribute to the codebase, you must fork the repository on github, than clone it locally with:

```shell
git clone https://github.com/<username>/csharp-metar-decoder
```

Install all the dependencies using nuget :

```shell
nuget restore csharp-metar-decoder\
```

You're ready to launch the test suite with:

```shell
nunit-console.exe /xml:results.xml csharp-metar-decoder-tests\bin\debug\csharp-metar-decoder-tests.dll
```

This library is fully unit tested, and uses [nUnit]((http://nunit.org/)) to launch the tests.

Travis CI is used for continuous integration, which triggers tests for C# 3.5 for each push to the repo.



