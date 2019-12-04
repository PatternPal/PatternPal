using System;
using System.Collections.Generic;
using IDesign.Recognizers;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IDesign.Core
{
    public class RecognizerRunner
    {
        public static List<DesignPattern> designPatterns = new List<DesignPattern>
        {
            new DesignPattern("Singleton", new SingletonRecognizer()),
            new DesignPattern("Factory Method", new FactoryRecognizer())
        };
        public Dictionary<string, EntityNode> EntityNodes =
            new Dictionary<string, EntityNode>();

        public event EventHandler<RecognizerProgress> OnProgressUpdate;

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
                var tree = FileManager.MakeStringFromFile(files[i]);
                var generateSyntaxTree = new GenerateSyntaxTree(tree, files[i], EntityNodes);

                ProgressUpdate((int) (i / (float) files.Count * 50f), "Reading file: " + files[i]);
            }

            //Make relations
            var determineRelations = new DetermineRelations(EntityNodes);
            determineRelations.GetEdgesOfEntityNode();

            var j = 0;
            foreach (var node in EntityNodes.Values)
            {
                j++;
                ProgressUpdate((int) (j / (float) files.Count * 50f + 50), "Scanning class: " + node.GetName());
                foreach (var pattern in patterns)
                    results.Add(new RecognitionResult
                    {
                        Result = pattern.Recognizer.Recognize(node),
                        EntityNode = node,
                        Pattern = pattern
                    });
            }
            return results;
        }

        private void ProgressUpdate(int percentage, string status)
        {
            OnProgressUpdate?.Invoke(this, new RecognizerProgress
            {
                CurrentPercentage = percentage,
                Status = status
            });
        }
    }
}
