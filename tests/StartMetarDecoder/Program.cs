using CommandLine;
using CommandLine.Text;
using MetarDecoder;
using MetarDecoder.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace StartMetarDecoder
{
    class Program
    {
        class Options
        {
            [Option("Metar", Required = true, HelpText = "Path to the XML Configuration File.")]
            public string Metar { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }

        static void Main(string[] args)
        {
            var MetarDecoder = new MetarDecoder.Decoder();
            MetarDecoder.SetStrictParsing(true);

            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
                var decodedMetar = Decoder.ParseWithMode(options.Metar);
                Display(decodedMetar);
            }
        }

        private static void Display(object o, string prefix = "")
        {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(o))
            {
                var name = descriptor.Name;
                var value = descriptor.GetValue(o);

                if (value is ReadOnlyCollection<Exception>)
                {
                    Console.WriteLine($"{name}.Count={(value as ReadOnlyCollection<Exception>).Count}");
                }
                else if (value is SurfaceWind)
                {
                    var surfaceWind = value as SurfaceWind;
                    Display(surfaceWind, prefix + "SurfaceWind.");
                }
                else if (value is Visibility)
                {
                    var visibility = value as Visibility;
                    Display(visibility, prefix + "Visibility.");
                }
                else if (value is List<RunwayVisualRange>)
                {
                    var listRunwayVisualRange = value as List<RunwayVisualRange>;
                    foreach (var runwayVisualRange in listRunwayVisualRange)
                    {
                        Display(runwayVisualRange, prefix + "RunwayVisualRange.");
                    }
                }
                else if (value is List<WeatherPhenomenon>)
                {
                    var listPresentWeather = value as List<WeatherPhenomenon>;
                    foreach (var presentWeather in listPresentWeather)
                    {
                        Display(presentWeather, prefix + "PresentWeather.");
                    }
                }
                else if (value is List<CloudLayer>)
                {
                    var listCloud = value as List<CloudLayer>;
                    foreach (var cloud in listCloud)
                    {
                        Display(cloud, prefix + "Cloud.");
                    }
                }
                else
                {
                    Console.WriteLine($"{prefix}{name}={value?.ToString()}");
                }
            }
        }
    }
}
