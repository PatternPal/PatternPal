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
            const string path = @"C:\Users\Shanna\source\repos\DesignPatternRecognizer\IDesign\IDesign.Core";
            ReadFiles readFiles = new ReadFiles();
            readFiles.GetFilesFromDirectory(path);

            for (int i = 3; i < readFiles.Files.Count; i++)
            {
                string file = readFiles.MakeStringFromFile(readFiles.Files[i]);
                GenerateSyntaxTree generateSyntaxTree = new GenerateSyntaxTree(file);

                generateSyntaxTree.GetAllClassesOfFile();
                generateSyntaxTree.GetAllInterfacesOfFile();
                generateSyntaxTree.GetAllConstructorsOfAClass();
                generateSyntaxTree.GetAllMethodsOfAClass();
                generateSyntaxTree.GetAllPropertiesOfAClass();
                generateSyntaxTree.GetAllFieldsOfAClass();
                
                foreach(var z in generateSyntaxTree.EntityNodes)
                {
                    foreach(var field in z.FieldDeclarationSyntaxList)
                    {
                        Console.WriteLine(field);

                    }
                }
               
                //foreach(var member in generateSyntaxTree.EntityNodes)
                //{
                //    Console.WriteLine("CLASSNAME");
                //    Console.WriteLine(member.InterfaceOrClassNode.Identifier);
                //    Console.WriteLine();

                //    foreach (var k in member.PropertyDeclarationSyntaxList)
                //    {
                //        Console.WriteLine(k);
                //        Console.WriteLine("------");
                //    }
                        
                 
                   

                //}
            }
        }
    }
}
