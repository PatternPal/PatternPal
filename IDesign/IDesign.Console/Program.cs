using IDesign.Core;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IDesign.ConsoleApp
{
    internal class Program
    {
        /// <summary>
        ///     Prints the parses commandline input and starts the runner
        /// </summary>
        /// <param name="args">Takes in commandline options and .cs files</param>
        private static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                Console.WriteLine("No arguments or files specified please confront --help");
                Console.ReadKey();
                return;
            }

            var showHelp = false;
            var selectedFiles = new List<string>();
            var selectedPatterns = new List<DesignPattern>();
            var runner = new RecognizerRunner();
            var manager = new FileManager();
            var designPatterns = RecognizerRunner.designPatterns;

            var options = new OptionSet
            {
                {"h|help", "shows this message and exit", v => showHelp = v != null}
            };

            //Add design patterns as specifiable option
            foreach (var pattern in designPatterns)
                options.Add(pattern.Name, "includes " + pattern.Name, v => selectedPatterns.Add(pattern));

            var arguments = options.Parse(args);

            if (showHelp)
            {
                ShowHelp(options);
                Console.ReadKey();
                return;
            }

            selectedFiles = (from a in arguments where a.EndsWith(".cs") && a.Length > 3 select a).ToList();
            
            foreach (string arg in arguments)
            {
                if (Directory.Exists(arg))
                    selectedFiles.AddRange(manager.GetAllCsFilesFromDirectory(arg));
            }

            if (selectedFiles.Count == 0)
            {
                Console.WriteLine("No files specified!");
                Console.ReadKey();
                return;
            }

            //When no specific pattern is chosen, select all
            if (selectedPatterns.Count == 0) selectedPatterns = designPatterns;

            Console.WriteLine("Selected files:");

            foreach (var file in selectedFiles) Console.WriteLine(file);

            Console.WriteLine("\nSelected patterns:");

            foreach (var pattern in selectedPatterns) Console.WriteLine(pattern.Name);

            PrintResults(runner.Run(selectedFiles, selectedPatterns));

            Console.ReadKey();
        }

        /// <summary>
        ///     Prints a message on how to use this program and all possible options
        /// </summary>
        /// <param name="options">All commandline options</param>
        private static void ShowHelp(OptionSet options)
        {
            Console.WriteLine("Usage: idesign [INPUT] [OPTIONS]");
            Console.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Out);
        }

        /// <summary>
        /// Prints results of RecognizerRunner.Run
        /// </summary>
        /// <param name="results">A List of RecognitionResult</param>
        private static void PrintResults(List<RecognitionResult> results)
        {
            Console.WriteLine("\nResults:");

            for (int i = 0; i < results.Count; i++)
            {
                Console.Write($"{i}) {results[i].EntityNode.GetName()} | {results[i].Pattern.Name}: ");

                PrintScore(results[i].Result.GetScore());

                Console.ForegroundColor = ConsoleColor.Red;

                foreach (var suggestion in results[i].Result.GetSuggestions())
                    Console.WriteLine($"\t- {suggestion.GetMessage()}");

                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        /// <summary>
        /// Prints the score with a color depending on the score
        /// </summary>
        /// <param name="score"></param>
        private static void PrintScore(int score)
        {
            if (score <= 33)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (score <= 66)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else
                Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine(score);

            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}