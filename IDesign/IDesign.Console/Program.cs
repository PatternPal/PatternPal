using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IDesign.CommonResources;
using IDesign.Core;
using IDesign.Core.Models;
using IDesign.Recognizers.Abstractions;
using Microsoft.CodeAnalysis;
using NDesk.Options;

namespace IDesign.ConsoleApp
{
    internal static class Program
    {
        /// <summary>
        ///     Prints the parses commandline input and starts the runner
        /// </summary>
        /// <param name="args">Takes in commandline options and .cs files</param>
        private static void Main(string[] args)
        {
            var designPatternsList = RecognizerRunner.designPatterns;
            var showHelp = false;
            var selectedFiles = new List<string>();
            var selectedDirectories = new List<string>();
            var selectedPatterns = new List<DesignPattern>();
            var fileManager = new FileManager();
            var recognizerRunner = new RecognizerRunner();

            var options = new OptionSet
            {
                {"h|help", "shows this message and exit", v => showHelp = v != null}
            };

            //Add design patterns as specifiable option
            foreach (var pattern in designPatternsList)
            {
                options.Add(pattern.Name.Replace(" ", "_"), "includes " + pattern.Name, v => selectedPatterns.Add(pattern));
            }
            
            if (args.Length <= 0)
            {
                Console.WriteLine("No arguments or files specified\n");
                ShowHelpMessage(options); 
                Console.WriteLine("\nYou can write the argument in the console:");
                string input = Console.ReadLine();
                if (input == null) return;
                
                args = SplitCommandLine(input).ToArray();
                if (args.Length <= 0) return;
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
                Console.ReadKey();
                return;
            }

            //When no specific pattern is chosen, select all
            if (selectedPatterns.Count == 0)
            {
                selectedPatterns = designPatternsList;
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

            recognizerRunner.OnProgressUpdate += (sender, progress) =>
             DrawTextProgressBar(progress.Status, progress.CurrentPercentage, 100);

            recognizerRunner.CreateGraph(selectedFiles);
            var results = recognizerRunner.Run(selectedPatterns);

            PrintResults(results, selectedDirectories);

            Console.ReadKey();
        }

        private static void ShowHelpMessage(OptionSet options)
        {
            Console.WriteLine("Usage: idesign [INPUT] [OPTIONS]");
            Console.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Out);
        }

        private static void PrintResults(List<RecognitionResult> results, List<string> selectedDirectories)
        {
            Console.WriteLine("\nResults:");

            results = results.Where(x => x.Result.GetScore() >= 50).ToList();

            for (var i = 0; i < results.Count; i++)
            {
                var name = results[i].EntityNode.GetName();

                //If a directory was selected, show from which subdirectories the entitynode originated
                if (selectedDirectories.Count > 0)
                {
                    foreach (var item in selectedDirectories)
                    {
                        if (results[i].EntityNode.GetSourceFile().Contains(item))
                        {
                            name = results[i].EntityNode.GetSourceFile().Replace(item, "");
                            break;
                        }
                    }
                }

                Console.Write($"{i}) {name} | {results[i].Pattern.Name}: ");

                PrintScore(results[i].Result.GetScore());

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

            Console.WriteLine(new string('\t', depth) + symbol + $"{ResourceUtils.ResultToString(result)} | {result.GetScore()}p / {result.GetTotalChecks()}p");

            foreach (var child in result.GetChildFeedback())
            {
                PrintResult(child, depth + 1);
            }
        }

        private static void PrintScore(int score)
        {
            Console.ForegroundColor = score < 40 ? ConsoleColor.Red : score < 80 ? ConsoleColor.Yellow : ConsoleColor.Green;
            Console.WriteLine(score);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void DrawTextProgressBar(string stepDescription, int progress, int total)
        {
            int totalChunks = 30;

            //draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = totalChunks + 1;
            Console.Write("]"); //end
            Console.CursorLeft = 1;

            double pctComplete = Convert.ToDouble(progress) / total;
            int numChunksComplete = Convert.ToInt16(totalChunks * pctComplete);

            //draw completed chunks
            Console.BackgroundColor = ConsoleColor.Green;
            Console.Write("".PadRight(numChunksComplete));

            //draw incomplete chunks
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write("".PadRight(totalChunks - numChunksComplete));

            //draw totals
            Console.CursorLeft = totalChunks + 5;
            Console.BackgroundColor = ConsoleColor.Black;

            string output = progress.ToString() + " of " + total.ToString();
            //pad the output so when changing from 3 to 4 digits we avoid text shifting
            Console.Write(output.PadRight(15) + stepDescription);
        }

        public static IEnumerable<string> SplitCommandLine(string commandLine)
        {
            bool inQuotes = false;

            return commandLine.Split(c =>
                {
                    if (c == '\"')
                        inQuotes = !inQuotes;

                    return !inQuotes && c == ' ';
                })
                .Select(arg => arg.Trim().TrimMatchingQuotes('\"'))
                .Where(arg => !string.IsNullOrEmpty(arg));
        }
        
        public static IEnumerable<string> Split(this string str, 
            Func<char, bool> controller)
        {
            int nextPiece = 0;

            for (int c = 0; c < str.Length; c++)
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
            if ((input.Length >= 2) && 
                (input[0] == quote) && (input[input.Length - 1] == quote))
                return input.Substring(1, input.Length - 2);

            return input;
        }
    }
}
