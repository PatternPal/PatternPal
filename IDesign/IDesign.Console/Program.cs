using IDesign.Core;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IDesign.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> patterns = new List<string> { "Singleton", "Factory method"};
            const string path = @"C:\Users\Shanna\source\repos\DesignPatternRecognizer\IDesign\IDesign.Core";
           
            RecognizerRunner recognizerRunner = new RecognizerRunner();
            ReadFiles readFiles = new ReadFiles();

            readFiles.GetFilesFromDirectory(path);

            recognizerRunner.Run(readFiles.Files, patterns);

         
        }
    }
}
