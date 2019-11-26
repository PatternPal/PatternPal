using IDesign.Core;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IDesign.ConsoleApp
{
    class Program
    {
        public static List<DesignPattern> designPatterns = new List<DesignPattern> {
            new DesignPattern("Singleton"),
            new DesignPattern("State"),
            new DesignPattern("Strategy"),
            new DesignPattern("Factory"),
            new DesignPattern("Decorator"),
            new DesignPattern("Adapter"),
        };

        /// <summary>
        /// This is the main function, it takes is options and .files, and prints a list of .cs files and specified design patterns
        /// </summary>
        /// <param name="args">Takes in commandline options and .cs files</param>
        static void Main(string[] args)
        {
            const string path = @"C:\Users\Shanna\source\repos\DesignPatternRecognizer\IDesign\IDesign.Core";

            RecognizerRunner recognizerRunner = new RecognizerRunner();
            FileManager readFiles = new FileManager();

            readFiles.GetFilesFromDirectory(path);
            recognizerRunner.Run(readFiles.Files, designPatterns);

            if (args.Length <= 0)
            {
                Console.WriteLine("No arguments or files specified please confront --help");
                Console.ReadKey();
                return;
            }

            bool showHelp = false;
            List<string> selectedFiles = new List<string>();
            List<DesignPattern> selectedPatterns = new List<DesignPattern>();

            var options = new OptionSet()
            {
                {"h|help", "shows this message and exit", v => showHelp = v != null },
            };

            //Add design patterns as specifiable option
            foreach (DesignPattern pattern in designPatterns)
            {
                options.Add(pattern.Name, "includes " + pattern.Name, v => selectedPatterns.Add(pattern));
            }

            List<string> arguments = options.Parse(args);
            
            if (showHelp)
            {
                ShowHelp(options);
                Console.ReadKey();
                return;
            }

            selectedFiles = (from a in arguments where a.EndsWith(".cs") && a.Length > 3 select a).ToList();

            if (selectedFiles.Count == 0)
            {
                Console.WriteLine("No files specified!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Selected files:");
            foreach (string file in selectedFiles)
            {
                Console.WriteLine(file);
            }

            //When no specific pattern is chosen, select all
            if (selectedPatterns.Count == 0)
            {
                selectedPatterns = designPatterns;
            }

            Console.WriteLine("\nSelected patterns:");
            foreach (DesignPattern pattern in selectedPatterns)
            {
                Console.WriteLine(pattern.Name);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Prints a message on how to use this program and all possible options
        /// </summary>
        /// <param name="options">All commandline options</param>
        static void ShowHelp(OptionSet options)
        {
            Console.WriteLine("Usage: idesign [INPUT] [OPTIONS]");
            Console.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Out);
        }
    }
}
