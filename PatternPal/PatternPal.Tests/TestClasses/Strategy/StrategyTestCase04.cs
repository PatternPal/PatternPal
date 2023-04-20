using System;
using System.Collections.Generic;
using System.Text;

namespace PatternPal.Tests.TestClasses.Strategy
{
    /* Pattern:              Strategy
         * Original code source: https://dotnettutorials.net/lesson/strategy-design-pattern/
         * 
         * 
         * Requirements to fullfill the pattern:
         *         Strategy interface
         *            ✓  a) is an interface	/ abstract class
         *            ✓  b) has declared a method
         *            ✓        1) if the class is an abstract instead of an interface the method has to be an abstract method
         *            ✓  c) is used by another class
         *            ✓  d) is implemented / inherited by at least one other class
         *               e) is implemented / inherited by at least two other classes
         *         Concrete strategy
         *            ✓  a) is an implementation of the Strategy interface
         *            ✓  b) if the class is used, it must be used via the context class
         *            ✓  c) if the class is not used it should be used via the context class
         *            ✓  d) is stored in the context class
         *         Context
         *            ✓  a) has a private field or property that has a Strategy class as type 
         *               b) has a function setStrategy() to set the non-public field / property with parameter of type Strategy
         *            ✓  c) has a function useStrategy() to execute the strategy. 
         *         Client
         *            ✓  a) has created an object of the type ConcreteStrategy
         *               b) has used the setStrategy() in the Context class to store the ConcreteStrategy object
         *            ✓  c) has executed the ConcreteStrategy via the Context class
         */

    //Strategy interface
    public interface ICompression
    {
        void CompressFolder(string compressedArchiveFileName);
    }

    //Concrete strategy
    public class RarCompression : ICompression
    {
        public void CompressFolder(string compressedArchiveFileName)
        {
            Console.WriteLine("Folder is compressed using Rar approach: '" + compressedArchiveFileName
                                                                           + ".rar' file is created");
        }
    }

    //Concrete strategy
    //public class ZipCompression : ICompression
    //{
    //    public void CompressFolder(string compressedArchiveFileName)
    //    {
    //        Console.WriteLine("Folder is compressed using zip approach: '" + compressedArchiveFileName
    //                                                                       + ".zip' file is created");
    //    }
    //}

    //Context
    public class CompressionContext
    {
        private ICompression Compression;
        
        public CompressionContext(ICompression Compression)
        {
            this.Compression = Compression;
        }
        public void CreateArchive(string compressedArchiveFileName)
        {
            Compression.CompressFolder(compressedArchiveFileName);
        }
    }

    //Client
    file class Program
    {
        static void EntryPoint(string[] args)
        {
            CompressionContext ctx = new CompressionContext(new RarCompression());
            ctx.CreateArchive("DotNetDesignPattern");
            Console.Read();
        }
    }
}
