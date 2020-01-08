using System;
using System.Collections.Generic;
using System.IO;
using IDesign.Core.Models;
using IDesign.Recognizers;
using Microsoft.CodeAnalysis;

namespace IDesign.Core
{
    public class RecognizerRunner
    {
        public static List<DesignPattern> designPatterns = new List<DesignPattern>
        {
            new DesignPattern("Singleton", new SingletonRecognizer(), "Singleton_pattern"),
            new DesignPattern("Factory Method", new FactoryMethodRecognizer(), "Factory_method_pattern"),
            new DesignPattern("Decorator", new DecoratorRecognizer(), "Decorator_pattern"),
            new DesignPattern("State", new StateRecognizer(), "State_pattern"),
            new DesignPattern("Strategy", new StrategyRecognizer(), "Strategy_pattern"),
            new DesignPattern("Adapter", new AdapterRecognizer(), "Adapter_pattern"),
            new DesignPattern("Observer", new ObserverRecognizer(), "Observer_pattern"),
        };

        public Dictionary<string, EntityNode> EntityNodes =
            new Dictionary<string, EntityNode>();

        public event EventHandler<RecognizerProgress> OnProgressUpdate;

        public Dictionary<SyntaxTree, string> CreateGraph(List<string> files)
        {
            var syntaxTreeSources = new Dictionary<SyntaxTree, string>();

            for (var i = 0; i < files.Count; i++)
            {
                var tree = FileManager.MakeStringFromFile(files[i]);
                var generateSyntaxTree = new SyntaxTreeGenerator(tree, files[i], EntityNodes);
                syntaxTreeSources.Add(generateSyntaxTree.Tree, files[i]);
                ProgressUpdate((int)(i / (float)files.Count * 50f), "Reading file: " + Path.GetFileName(files[i]));
            }

            //Make relations
            var determineRelations = new DetermineRelations(EntityNodes);
            determineRelations.CreateEdgesOfEntityNode();
            return syntaxTreeSources;
        }

        /// <summary>
        ///     Function that should be called to generate a syntax tree
        /// </summary>
        /// <param name="files"></param>
        /// <param name="patterns"></param>
        /// <returns></returns>
        public List<RecognitionResult> Run(List<DesignPattern> patterns)
        {
            var results = new List<RecognitionResult>();
            var j = 0;
            foreach (var node in EntityNodes.Values)
            {
                j++;
                ProgressUpdate((int)(j / (float)EntityNodes.Count * 50f + 50), "Scanning class: " + node.GetName());
                foreach (var pattern in patterns)
                {
                    results.Add(new RecognitionResult
                    {
                        Result = pattern.Recognizer.Recognize(node),
                        EntityNode = node,
                        FilePath = node.SourceFile,
                        Pattern = pattern
                    });
                }
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