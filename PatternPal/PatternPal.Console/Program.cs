using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using PatternPal.CommonResources;
using PatternPal.Core;
using PatternPal.Core.Models;
using PatternPal.Recognizers.Abstractions;
using NDesk.Options;

namespace PatternPal.ConsoleApp
{
    internal static class Program
    {
        /// <summary>
        ///     Prints the parses commandline input and starts the runner
        /// </summary>
        /// <param name="args">Takes in commandline options and .cs files</param>
        private static void Main(string[] args)
        {
            var designPatternsList = RecognizerRunner.DesignPatterns;
            var showHelp = false;
            var selectedFiles = new List<string>();
            var selectedDirectories = new List<string>();
            var selectedPatterns = new List<DesignPattern>();
            var fileManager = new FileManager();
            var recognizerRunner = new RecognizerRunner();

            var options = new OptionSet {{"h|help", "shows this message and exit", v => showHelp = v != null}};

            //Add design patterns as specifiable option
            foreach (var pattern in designPatternsList)
            {
                options.Add(
                    pattern.Name.Replace(" ", "_"), "includes " + pattern.Name, v => selectedPatterns.Add(pattern)
                );
            }

            if (args.Length <= 0)
            {
                Console.WriteLine("No arguments or files specified\n");
                ShowHelpMessage(options);
                Console.WriteLine("\nYou can write the argument in the console:");
                var input = Console.ReadLine();
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

            var arguments = options.Parse(args);

            if (showHelp)
            {
                ShowHelpMessage(options);
                return;
            }

            selectedFiles = (from a in arguments where a.EndsWith(".cs") && a.Length > 3 select a).ToList();

            foreach (var arg in from arg in arguments
                     where Directory.Exists(arg)
                     select arg)
            {
                selectedFiles.AddRange(fileManager.GetAllCSharpFilesFromDirectory(arg));
            }

            selectedDirectories = (from dir in arguments where Directory.Exists(dir) select dir).ToList();

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

            foreach (var file in selectedFiles)
            {
                Console.WriteLine(" - " + file);
            }

            Console.WriteLine("\nSelected patterns:");

            foreach (var pattern in selectedPatterns)
            {
                Console.WriteLine(" - " + pattern.Name);
            }

            Console.WriteLine();
            recognizerRunner.OnProgressUpdate += (sender, progress) =>
                DrawTextProgressBar(progress.Status, progress.CurrentPercentage, 100);
            
            recognizerRunner.CreateGraph(selectedFiles);
            var results = recognizerRunner.Run(selectedPatterns);

            Console.WriteLine();
            PrintResults(results, selectedDirectories);
        }

        private static void ShowHelpMessage(OptionSet options)
        {
            Console.WriteLine("Usage: patterpal [INPUT] [OPTIONS]");
            Console.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Out);
        }

        private static void PrintResults(List<RecognitionResult> results, List<string> selectedDirectories)
        {
            Console.WriteLine("\nResults:");

            results = results.Where(x => x.Result.GetScore() >= 80).ToList();

            for (var i = 0; i < results.Count; i++)
            {
                var name = results[i].EntityNode.GetName();

                //If a directory was selected, show from which subdirectories the entitynode originated
                if (selectedDirectories.Count > 0)
                {
                    foreach (var item in selectedDirectories)
                    {
                        if (results[i].EntityNode.GetRoot().GetSource().Contains(item))
                        {
                            name = results[i].EntityNode.GetRoot().GetSource().Replace(item, "");
                            break;
                        }
                    }
                }

                Console.WriteLine($"{i}) {name} | {results[i].Pattern.Name}");
                Trace.WriteLine($"{i}) {name} | {results[i].Pattern.Name}");

                Console.ForegroundColor = ConsoleColor.Red;

                foreach (var result in results[i].Result.GetResults())
                {
                    PrintResult(result, 1);
                }

                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public static void PrintResult(ICheckResult result, int depth)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            var symbol = "X";

            switch (result.GetFeedbackType())
            {
                case FeedbackType.SemiCorrect:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    symbol = "-";
                    break;
                case FeedbackType.Correct:
                    Console.ForegroundColor = ConsoleColor.Green;
                    symbol = "✓";
                    break;
            }

            var text = new string('\t', depth) + symbol +
                       $" {ResourceUtils.ResultToString(result)}";

            Console.WriteLine(text);
            Trace.WriteLine(text);

            foreach (var child in result.GetChildFeedback())
            {
                PrintResultRecursive(child, depth + 1);
            }
        }

        private static void PrintResultRecursive(ICheckResult result, int depth)
        {
            if (result.IsHidden)
            {
                foreach (var sub in result.GetChildFeedback())
                {
                    PrintResultRecursive(sub, depth);
                }
            }
            else
            {
                PrintResult(result, depth);
            }
        }

        private static void PrintScore(int score)
        {
            Console.ForegroundColor =
                score < 40 ? ConsoleColor.Red : score < 80 ? ConsoleColor.Yellow : ConsoleColor.Green;
            Console.WriteLine(score);
            Trace.WriteLine(score);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void DrawTextProgressBar(string stepDescription, int progress, int total)
        {
            var totalChunks = 30;

            //draw empty progress bar
            Console.Write('\r');
            Console.Write(@"[");

            var pctComplete = Convert.ToDouble(progress) / total;
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

            var output = progress + " of " + total;
            //pad the output so when changing from 3 to 4 digits we avoid text shifting
            Console.Write(output.PadRight(15) + stepDescription);
            Console.Write(new string(' ', Console.WindowWidth - Console.CursorLeft - 1));
        }

        public static IEnumerable<string> SplitCommandLine(string commandLine)
        {
            var inQuotes = false;

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

        public static IEnumerable<string> Split(
            this string str,
            Func<char, bool> controller
        )
        {
            var nextPiece = 0;

            for (var c = 0; c < str.Length; c++)
            {
                if (controller(str[c]))
                {
                    yield return str.Substring(nextPiece, c - nextPiece);
                    nextPiece = c + 1;
                }
            }

            yield return str.Substring(nextPiece);
        }

        public static string TrimMatchingQuotes(this string input, char quote)
        {
            if (input.Length >= 2 &&
                input[0] == quote && input[input.Length - 1] == quote)
            {
                return input.Substring(1, input.Length - 2);
            }

            return input;
        }
    }
}
