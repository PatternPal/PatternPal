#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

using NDesk.Options;

using PatternPal.Core;
using PatternPal.Core.Models;

#endregion

namespace PatternPal.ConsoleApp
{
    internal static class Program
    {
        /// <summary>
        ///     Prints the parses commandline input and starts the runner
        /// </summary>
        /// <param name="args">Takes in commandline options and .cs files</param>
        private static void Main(
            string[ ] args)
        {
            DesignPattern[ ] designPatternsList = DesignPattern.SupportedPatterns;
            bool showHelp = false;
            List< DesignPattern > selectedPatterns = new();
            FileManager fileManager = new();

            OptionSet options = new()
                                {
                                    {
                                        "h|help", "shows this message and exit", v => showHelp = v != null
                                    }
                                };

            //Add design patterns as specifiable option
            foreach (DesignPattern pattern in designPatternsList)
            {
                options.Add(
                    pattern.Name.Replace(
                        " ",
                        "_"),
                    "includes " + pattern.Name,
                    v => selectedPatterns.Add(pattern)
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

            List< string > selectedDirectories = (from dir in arguments where Directory.Exists(dir) select dir).ToList();

            if (selectedFiles.Count == 0)
            {
                Console.WriteLine("No files specified!");
                return;
            }

            //When no specific pattern is chosen, select all
            if (selectedPatterns.Count == 0)
            {
                selectedPatterns = designPatternsList.ToList();
            }

            Console.WriteLine("Selected files:");

            foreach (string file in selectedFiles)
            {
                Console.WriteLine(" - " + file);
            }

            Console.WriteLine("\nSelected patterns:");

            foreach (DesignPattern pattern in selectedPatterns)
            {
                Console.WriteLine(" - " + pattern.Name);
            }

            Console.WriteLine();

            RecognizerRunner recognizerRunner = new(
                selectedFiles,
                selectedPatterns );

            ICheckResult result = recognizerRunner.RunV2();
            if (result is null)
            {
                Console.WriteLine("No results.");
                return;
            }

            Console.WriteLine();
            PrintResults(
                result,
                selectedDirectories);
        }

        private static void ShowHelpMessage(
            OptionSet options)
        {
            Console.WriteLine("Usage: patterpal [INPUT] [OPTIONS]");
            Console.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Out);
        }

        private static void PrintResults(
            ICheckResult result,
            List< string > selectedDirectories)
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
                    }));
            Console.WriteLine();
        }

        public static void DrawTextProgressBar(
            string stepDescription,
            int progress,
            int total)
        {
            int totalChunks = 30;

            //draw empty progress bar
            Console.Write('\r');
            Console.Write(@"[");

            double pctComplete = Convert.ToDouble(progress) / total;
            int numChunksComplete = Convert.ToInt16(totalChunks * pctComplete);

            //draw completed chunks
            Console.BackgroundColor = ConsoleColor.Green;
            Console.Write("".PadRight(numChunksComplete));

            //draw incomplete chunks
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write("".PadRight(totalChunks - numChunksComplete));

            //draw totals
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(@"]     "); //end

            string output = progress + " of " + total;
            //pad the output so when changing from 3 to 4 digits we avoid text shifting
            Console.Write(output.PadRight(15) + stepDescription);
            Console.Write(
                new string(
                    ' ',
                    Console.WindowWidth - Console.CursorLeft - 1));
        }

        public static IEnumerable< string > SplitCommandLine(
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

        public static IEnumerable< string > Split(
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

            yield return str.Substring(nextPiece);
        }

        public static string TrimMatchingQuotes(
            this string input,
            char quote)
        {
            if (input.Length >= 2
                && input[ 0 ] == quote
                && input[ input.Length - 1 ] == quote)
            {
                return input.Substring(
                    1,
                    input.Length - 2);
            }

            return input;
        }
    }
}
