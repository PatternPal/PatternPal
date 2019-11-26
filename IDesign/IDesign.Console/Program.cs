using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using IDesign.Recognizers.Abstractions;
using IDesign.Core;
using NDesk.Options;

namespace IDesign.ConsoleApp
{
    internal class Program
    {
        /// <summary>
        ///     This is the main function, it takes is options and .files, and prints a list of .cs files and specified design
        ///     patterns
        /// </summary>
        /// <param name="args">Takes in commandline options and .cs files</param>
        private static void Main(string[] args)
        {
            List<DesignPattern> designPatterns = RecognizerRunner.designPatterns;

            if (args.Length <= 0)
            {
                Console.WriteLine("No arguments or files specified please confront --help");
                Console.ReadKey();
                return;
            }

            var showHelp = false;
            var selectedFiles = new List<string>();
            var selectedPatterns = new List<DesignPattern>();

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

            FileManager manager = new FileManager();

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

            RecognizerRunner recognizerRunner = new RecognizerRunner();

            List<IResult> results = recognizerRunner.Run(selectedFiles, designPatterns);

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
    }
}