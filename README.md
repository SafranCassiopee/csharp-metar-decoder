
C# METAR decoder
=================
A .NET Standard library to decode METAR strings, fully unit tested (100% code coverage)

This is largely based on [csharp-metar-decoder](https://github.com/SafranCassiopee/csharp-metar-decoder)

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

This library package only requires .NET Standard 2.0

Usage:

```shell
StartMetarDecoder.exe --Metar "LFPO 231027Z AUTO 24004G09MPS 2500 1000NW R32/0400 R08C/0004D +FZRA VCSN //FEW015 17/10 Q1009 REFZRA WS R03"
```

If you want to integrate the library easily in your project, you should consider using the official nuget package available from https://www.nuget.org/.

```
nuget install metar-decoder
```

It is not mandatory though.

Setup
-----

- With nuget.exe *(recommended)*

From the Package Manager Console in Visual Studio

```shell
nuget install metar-decoder
```

Add a reference to the library, then add the following using directives:

```csharp
using MetarDecoder;
using MetarDecoder.Entity;
```

Usage
-----
TODO

About Value objects
-------------------
TODO


About parsing errors
--------------------
TODO