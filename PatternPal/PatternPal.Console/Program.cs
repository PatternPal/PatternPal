#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using NDesk.Options;

using PatternPal.Core;
using PatternPal.Core.Recognizers;
using PatternPal.Protos;

#endregion

namespace PatternPal.ConsoleApp;

internal static class Program
{
    /// <summary>
    ///     Prints the parses commandline input and starts the runner
    /// </summary>
    /// <param name="args">Takes in commandline options and .cs files</param>
    private static void Main(
        string[ ] args)
    {
        IDictionary< Recognizer, IRecognizer > supportedRecognizers = RecognizerRunner.SupportedRecognizers;
        bool showHelp = false;
        List< IRecognizer > selectedRecognizers = new();
        FileManager fileManager = new();

        OptionSet options = new()
                            {
                                {
                                    "h|help", "shows this message and exit", v => showHelp = v != null
                                }
                            };

        //Add design patterns as specifiable option
        foreach (IRecognizer recognizer in supportedRecognizers.Values)
        {
            options.Add(
                recognizer.Name.Replace(
                    " ",
                    "_"),
                "includes " + recognizer.Name,
                // This closure is not stored, so accessing the captured variable is not an issue.
                // ReSharper disable once AccessToModifiedClosure
                _ => selectedRecognizers.Add(recognizer)
            );
        }

        if (args.Length <= 0)
        {
            Console.WriteLine("No arguments or files specified\n");
            ShowHelpMessage(options);
            Console.WriteLine("\nYou can write the argument in the console:");
            string input = Console.ReadLine();
            if (input == null)
            {
                return;
            }

            args = SplitCommandLine(input).ToArray();
            if (args.Length <= 0)
            {
                return;
            }
        }

        List< string > arguments = options.Parse(args);

        if (showHelp)
        {
            ShowHelpMessage(options);
            return;
        }

        List< string > selectedFiles = (from a in arguments where a.EndsWith(".cs") && a.Length > 3 select a).ToList();

        foreach (string arg in from arg in arguments
            where Directory.Exists(arg)
            select arg)
        {
            selectedFiles.AddRange(fileManager.GetAllCSharpFilesFromDirectory(arg));
        }

        if (selectedFiles.Count == 0)
        {
            Console.WriteLine("No files specified!");
            return;
        }

        //When no specific pattern is chosen, select all
        if (selectedRecognizers.Count == 0)
        {
            selectedRecognizers = supportedRecognizers.Values.ToList();
        }

        Console.WriteLine("Selected files:");

        foreach (string file in selectedFiles)
        {
            Console.WriteLine(" - " + file);
        }

        Console.WriteLine("\nSelected patterns:");

        foreach (IRecognizer selectedRecognizer in selectedRecognizers)
        {
            Console.WriteLine(" - " + selectedRecognizer.Name);
        }

        Console.WriteLine();

        RecognizerRunner recognizerRunner = new(
            selectedFiles,
            selectedRecognizers );

        IList< ICheckResult > result = recognizerRunner.Run();

        Console.WriteLine();
        PrintResults(
            result);
    }

    private static void ShowHelpMessage(
        OptionSet options)
    {
        Console.WriteLine("Usage: patterpal [INPUT] [OPTIONS]");
        Console.WriteLine("Options:");
        options.WriteOptionDescriptions(Console.Out);
    }

    private static void PrintResults(
        IList< ICheckResult > result)
    {
        Console.WriteLine("\nResults:");

        Console.WriteLine(
            JsonSerializer.Serialize(
                result,
                new JsonSerializerOptions
                {
                    Converters =
                    {
                        new JsonStringEnumConverter()
                    },
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true,
                    // NOTE: Workaround to ignore the required `ICheckResult.Check` property.
                    // See: https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/required-properties
                    TypeInfoResolver = new DefaultJsonTypeInfoResolver
                                       {
                                           Modifiers =
                                           {
                                               static ti =>
                                               {
                                                   if (ti.Kind != JsonTypeInfoKind.Object)
                                                   {
                                                       return;
                                                   }

                                                   foreach (JsonPropertyInfo propertyInfo in ti.Properties)
                                                   {
                                                       propertyInfo.IsRequired = false;
                                                   }
                                               }
                                           }
                                       }
                }));
        Console.WriteLine();
    }

    private static IEnumerable< string > SplitCommandLine(
        string commandLine)
    {
        bool inQuotes = false;

        return commandLine.Split(
                              c =>
                              {
                                  if (c == '\"')
                                  {
                                      inQuotes = !inQuotes;
                                  }

                                  return !inQuotes && c == ' ';
                              }
                          )
                          .Select(arg => arg.Trim().TrimMatchingQuotes('\"'))
                          .Where(arg => !string.IsNullOrEmpty(arg));
    }

    private static IEnumerable< string > Split(
        this string str,
        Func< char, bool > controller
    )
    {
        int nextPiece = 0;

        for (int c = 0;
             c < str.Length;
             c++)
        {
            if (controller(str[ c ]))
            {
                yield return str.Substring(
                    nextPiece,
                    c - nextPiece);
                nextPiece = c + 1;
            }
        }

        yield return str[ nextPiece.. ];
    }

    private static string TrimMatchingQuotes(
        this string input,
        char quote)
    {
        return input.Length >= 2
               && input[ 0 ] == quote
               && input[ ^1 ] == quote
            ? input.Substring(
                1,
                input.Length - 2)
            : input;
    }
}
