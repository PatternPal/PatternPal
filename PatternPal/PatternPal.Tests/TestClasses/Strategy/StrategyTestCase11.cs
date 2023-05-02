using System;
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
         *            ✓  e) is implemented / inherited by at least two other classes
         *         Concrete strategy
         *            ✓  a) is an implementation of the Strategy interface
         *            ✓  b) if the class is used, it must be used via the context class
         *            ✓  c) if the class is not used it should be used via the context class
         *            ✓  d) is stored in the context class
         *         Context
         *               a) has a private field or property that has a Strategy class as type 
         *            ✓  b) has a function setStrategy() to set the non-public field / property with parameter of type Strategy
         *            ✓  c) has a function useStrategy() to execute the strategy. 
         *         Client
         *            ✓  a) has created an object of the type ConcreteStrategy
         *            ✓  b) has used the setStrategy() in the Context class to store the ConcreteStrategy object
         *            ✓  c) has executed the ConcreteStrategy via the Context class
         */

    //Strategy interface
    public interface ICompression_
    {
        void CompressFolder(string compressedArchiveFileName);
    }

    //Concrete strategy
    public class RarCompression_ : ICompression_
    {
        public void CompressFolder(string compressedArchiveFileName)
        {
            Console.WriteLine("Folder is compressed using Rar approach: '" + compressedArchiveFileName
                                                                           + ".rar' file is created");
        }
    }

    //Concrete strategy
    public class ZipCompression_ : ICompression_
    {
        public void CompressFolder(string compressedArchiveFileName)
        {
            Console.WriteLine("Folder is compressed using zip approach: '" + compressedArchiveFileName
                                                                           + ".zip' file is created");
        }
    }

    //Context
    public class CompressionContext_
    {
        /*private */ICompression_ Compression;

        public CompressionContext_(ICompression_ Compression)
        {
            this.Compression = Compression;
        }
        public void SetStrategy(ICompression_ Compression)
        {
            this.Compression = Compression;
        }
        public void CreateArchive(string compressedArchiveFileName)
        {
            Compression.CompressFolder(compressedArchiveFileName);
        }
    }

    //Client
    class Program_
    {
        static void Main4(string[] args)
        {
            CompressionContext_ ctx = new CompressionContext_(new ZipCompression_());
            ctx.CreateArchive("DotNetDesignPattern");
            ctx.SetStrategy(new RarCompression_());
            ctx.CreateArchive("DotNetDesignPattern");
            Console.Read();
        }
    }
}
