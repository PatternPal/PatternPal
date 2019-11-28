using IDesign.Recognizers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace IDesign.Core
{
    public class RecognizerRunner
    {
        private readonly FileManager readFiles = new FileManager();
        public static List<DesignPattern> designPatterns = new List<DesignPattern>
        {
            new DesignPattern("Singleton", new SingletonRecognizer())
        };
        public Dictionary<TypeDeclarationSyntax, EntityNode> EntityNodes =
            new Dictionary<TypeDeclarationSyntax, EntityNode>();

        /// <summary>
        ///     Function that should be called to generate a syntax tree
        /// </summary>
        /// <param name="files"></param>
        /// <param name="patterns"></param>
        /// <returns></returns>
        public List<RecognitionResult> Run(List<string> files, List<DesignPattern> patterns)
        {
            var results = new List<RecognitionResult>();

            //loop over all files
            for (var i = 0; i < files.Count; i++)
            {
                var tree = readFiles.MakeStringFromFile(files[i]);
                var generateSyntaxTree = new GenerateSyntaxTree(tree, EntityNodes);
            }

            //Make relations
            var determineRelations = new DetermineRelations(EntityNodes);

            foreach (var pattern in patterns)
                foreach (var node in EntityNodes.Values)
                    results.Add(new RecognitionResult()
                    {
                        Result = pattern.Recognizer.Recognize(node),
                        EntityNode = node,
                        Pattern = pattern,
                    });

            return results;
        }
    }
}