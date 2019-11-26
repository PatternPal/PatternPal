using IDesign.Recognizers;
using IDesign.Recognizers.Abstractions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        public Dictionary<TypeDeclarationSyntax, EntityNode> EntityNodes = new Dictionary<TypeDeclarationSyntax, EntityNode>();
        private DetermineRelations DetermineRelations;

        /// <summary>
        /// Function that should be called to generate a syntax tree
        /// </summary>
        /// <param name="files"></param>
        /// <param name="patterns"></param>
        public List<IResult> Run(List<string> files, List<DesignPattern> patterns)
        {
            List<IResult> results = new List<IResult>();

            //loop over all files
            for (int i = 0; i < files.Count; i++)
            {
                var tree = readFiles.MakeStringFromFile(files[i]);
                GenerateSyntaxTree generateSyntaxTree = new GenerateSyntaxTree(tree, EntityNodes);
            }
            //Make relations
            DetermineRelations = new DetermineRelations(EntityNodes);

            foreach (var pattern in patterns)
                foreach (var node in EntityNodes.Values)
                    results.Add(pattern.Recognizer.Recognize(node));

            return results;
        }
    }
}
