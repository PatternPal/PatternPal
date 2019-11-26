using IDesign.Recognizers;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Core
{
    public class RecognizerRunner
    {
        private readonly FileManager readFiles = new FileManager();
        public static List<DesignPattern> designPatterns = new List<DesignPattern>
        {
            new DesignPattern("Singleton", new SingletonRecognizer())
        };

        /// <summary>
        /// Function that should be called to generate a syntax tree
        /// </summary>
        /// <param name="files"></param>
        /// <param name="patterns"></param>
        public void Run(List<string> files, List<DesignPattern> patterns)
        {
            //loop over all files
            for (int i = 0; i < files.Count; i++)
            {
                var tree = readFiles.MakeStringFromFile(files[i]);
                GenerateSyntaxTree generateSyntaxTree = new GenerateSyntaxTree(tree);

                //foreach file loop over all patterns and do stuff
                foreach (var pattern in patterns)
                    pattern.Recognizer.Recognize();
            }
        }
    }
}
