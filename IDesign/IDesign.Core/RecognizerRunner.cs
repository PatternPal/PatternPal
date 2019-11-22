using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Core
{
    public class RecognizerRunner
    {
        private GenerateSyntaxTree generateSyntaxTree;
        private ReadFiles readFiles = new ReadFiles();

        /// <summary>
        /// Function that should be called to generate a syntax tree
        /// </summary>
        /// <param name="files"></param>
        /// <param name="patterns"></param>
        public void Run(List<string> files, List<string> patterns)
        {
            //loop over all files
            for (int i = 0; i < files.Count; i++)
            {
                var tree = readFiles.MakeStringFromFile(files[i]);
                generateSyntaxTree = new GenerateSyntaxTree(tree);

                //foreach file loop over all patterns and do stuff
                for (int j = 0; j < patterns.Count; j++)
                {
                    //CODE GOES HERE
                }

            }

        }
    }
}
