using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IDesign.Core.Models;
using IDesign.Core.Resources;
using IDesign.Recognizers;
using IDesign.Recognizers.Recognizers;
using SyntaxTree;
using static IDesign.Core.Resources.DesignPatternNameResources;

namespace IDesign.Core
{
    public class RecognizerRunner
    {
        public static readonly List<DesignPattern> DesignPatterns = new List<DesignPattern>
        {
            new DesignPattern(Singleton, new SingletonRecognizer(), WikiPageResources.Singleton),
            new DesignPattern(FactoryMethod, new FactoryMethodRecognizer(), WikiPageResources.FactoryMethod),
            new DesignPattern(Decorator, new DecoratorRecognizer(), WikiPageResources.Decorator),
            new DesignPattern(State, new StateRecognizer(), WikiPageResources.State),
            new DesignPattern(Strategy, new StrategyRecognizer(), WikiPageResources.Strategy),
            new DesignPattern(Adapter, new AdapterRecognizer(), WikiPageResources.Adapter),
            new DesignPattern(Observer, new ObserverRecognizer(), WikiPageResources.Observer),
            new DesignPattern(Bridge, new BridgeRecognizer(), WikiPageResources.Bridge)
        };

        public SyntaxGraph Graph;

        public event EventHandler<RecognizerProgress> OnProgressUpdate;

        public SyntaxGraph CreateGraph(List<string> files)
        {
            Graph = new SyntaxGraph();

            for (var i = 0; i < files.Count; i++)
            {
                var content = FileManager.MakeStringFromFile(files[i]);
                Graph.AddFile(content, files[i]);
                ProgressUpdate((int)(i / (float)files.Count * 50f), "Reading file: " + Path.GetFileName(files[i]));
            }

            //Make relations
            Graph.CreateGraph();
            return Graph;
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

            if (Graph == null) return results;

            var entities = Graph.GetAll();

            if (entities == null) return results;
            
            foreach (var node in entities.Values)
            {
                j++;
                ProgressUpdate((int)((j / (float)entities.Count * 50f) + 50), "Scanning class: " + node.GetName());
                results.AddRange(
                    from pattern in patterns
                    select new RecognitionResult
                    {
                        Result = pattern.Recognizer.Recognize(node),
                        EntityNode = node,
                        FilePath = node.GetRoot().GetSource(),
                        Pattern = pattern
                    }
                );
            }

            return results;
        }

        private void ProgressUpdate(int percentage, string status)
        {
            OnProgressUpdate?.Invoke(this, new RecognizerProgress {CurrentPercentage = percentage, Status = status});
        }
    }
}
