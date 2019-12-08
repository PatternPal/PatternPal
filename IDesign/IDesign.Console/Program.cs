using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IDesign.Core;
using IDesign.Core.Models;
using NDesk.Options;

namespace IDesign.Console
{
    internal class Program
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
            var selectedPatterns = new List<DesignPattern>();
            var fileManager = new FileManager();
            var recognizerRunner = new RecognizerRunner();

            if (args.Length <= 0)
            {
                System.Console.WriteLine("No arguments or files specified please confront --help");
                System.Console.ReadKey();
                return;
            }

            var options = new OptionSet
            {
                {"h|help", "shows this message and exit", v => showHelp = v != null}
            };

            //Add design patterns as specifiable option
            foreach (var pattern in designPatternsList)
                options.Add(pattern.Name, "includes " + pattern.Name, v => selectedPatterns.Add(pattern));

            var arguments = options.Parse(args);

            if (showHelp)
            {
                ShowHelp(options);
                System.Console.ReadKey();
                return;
            }

            selectedFiles = (from a in arguments where a.EndsWith(".cs") && a.Length > 3 select a).ToList();

            foreach (var arg in arguments)
                if (Directory.Exists(arg))
                    selectedFiles.AddRange(fileManager.GetAllCsFilesFromDirectory(arg));

            if (selectedFiles.Count == 0)
            {
                System.Console.WriteLine("No files specified!");
                System.Console.ReadKey();
                return;
            }

            //When no specific pattern is chosen, select all
            if (selectedPatterns.Count == 0) selectedPatterns = designPatternsList;

            System.Console.WriteLine("Selected files:");

            foreach (var file in selectedFiles) System.Console.WriteLine(file);

            System.Console.WriteLine("\nSelected patterns:");

            foreach (var pattern in selectedPatterns) System.Console.WriteLine(pattern.Name);

            var results = recognizerRunner.Run(selectedFiles, selectedPatterns);

            PrintResults(results);

            System.Console.ReadKey();
        }

        /// <summary>
        ///     Prints a message on how to use this program and all possible options
        /// </summary>
        /// <param name="options">All commandline options</param>
        private static void ShowHelp(OptionSet options)
        {
            System.Console.WriteLine("Usage: idesign [INPUT] [OPTIONS]");
            System.Console.WriteLine("Options:");
            options.WriteOptionDescriptions(System.Console.Out);
        }

        /// <summary>
        ///     Prints results of RecognizerRunner.Run
        /// </summary>
        /// <param name="results">A List of RecognitionResult</param>
        private static void PrintResults(List<RecognitionResult> results)
        {
            System.Console.WriteLine("\nResults:");

            for (var i = 0; i < results.Count; i++)
            {
                System.Console.Write($"{i}) {results[i].EntityNode.GetName()} | {results[i].Pattern.Name}: ");

                PrintScore(results[i].Result.GetScore());

                System.Console.ForegroundColor = ConsoleColor.Red;

                foreach (var suggestion in results[i].Result.GetSuggestions())
                    System.Console.WriteLine($"\t- {suggestion.GetMessage()}");

                System.Console.ForegroundColor = ConsoleColor.White;
            }
        }

        /// <summary>
        ///     Prints the score with a color depending on the score
        /// </summary>
        /// <param name="score"></param>
        private static void PrintScore(int score)
        {
            if (score <= 33)
                System.Console.ForegroundColor = ConsoleColor.Red;
            else if (score <= 66)
                System.Console.ForegroundColor = ConsoleColor.Yellow;
            else
                System.Console.ForegroundColor = ConsoleColor.Green;

            System.Console.WriteLine(score);

            System.Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
