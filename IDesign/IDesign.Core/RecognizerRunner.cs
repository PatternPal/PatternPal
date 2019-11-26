using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Core
{
    public class RecognizerRunner
    {
        private readonly ReadFiles readFiles = new ReadFiles();
        private DetermineRelations DetermineRelations;

        public Dictionary<TypeDeclarationSyntax, EntityNode> EntityNodes =
            new Dictionary<TypeDeclarationSyntax, EntityNode>();

        /// <summary>
        ///     Function that should be called to generate a syntax tree
        /// </summary>
        /// <param name="files"></param>
        /// <param name="patterns"></param>
        public void Run(List<string> files, List<DesignPattern> patterns)
        {
            //loop over all files
            for (var i = 0; i < files.Count; i++)
            {
                var tree = readFiles.MakeStringFromFile(files[i]);
                var generateSyntaxTree = new GenerateSyntaxTree(tree, EntityNodes);

                //foreach file loop over all patterns and do stuff
                for (var j = 0; j < patterns.Count; j++)
                {
                    //CODE GOES HERE
                }
            }

            //Make relations
            DetermineRelations = new DetermineRelations(EntityNodes);
        }
    }
}