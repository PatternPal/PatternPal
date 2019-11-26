using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Core
{
    public class RecognizerRunner
    {
        private readonly ReadFiles readFiles = new ReadFiles();
        public Dictionary<TypeDeclarationSyntax, EntityNode> EntityNodes = new Dictionary<TypeDeclarationSyntax, EntityNode>();
        private DetermineRelations DetermineRelations;

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
                GenerateSyntaxTree generateSyntaxTree = new GenerateSyntaxTree(tree, EntityNodes);

                //foreach file loop over all patterns and do stuff
                for (int j = 0; j < patterns.Count; j++)
                {
                    //CODE GOES HERE
                }
            }
            //Make relations
            DetermineRelations = new DetermineRelations(EntityNodes);
        }

    }
}
