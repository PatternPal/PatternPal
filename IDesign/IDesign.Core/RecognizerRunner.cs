using System;
using System.Collections.Generic;
using System.IO;
using IDesign.Core.Models;
using System.Linq;
using IDesign.Recognizers.Recognizers;
using SyntaxTree;
using static IDesign.Core.Resources.DesignPatternNameResources;

namespace IDesign.Core {
    public class RecognizerRunner {
        public static readonly List<DesignPattern> DesignPatterns = new List<DesignPattern> {
            new DesignPattern(Singleton, new SingletonRecognizer(), Resources.WikiPageResources.Singleton),
            new DesignPattern(FactoryMethod, new FactoryMethodRecognizer(), Resources.WikiPageResources.FactoryMethod),
            new DesignPattern(Decorator, new DecoratorRecognizer(), Resources.WikiPageResources.Decorator),
            new DesignPattern(State, new StateRecognizer(), Resources.WikiPageResources.State),
            new DesignPattern(Strategy, new StrategyRecognizer(), Resources.WikiPageResources.Strategy),
            new DesignPattern(Adapter, new AdapterRecognizer(), Resources.WikiPageResources.Adapter),
            new DesignPattern(Observer, new ObserverRecognizer(), Resources.WikiPageResources.Observer),
        };

        public SyntaxGraph Graph;

        public event EventHandler<RecognizerProgress> OnProgressUpdate;

        public SyntaxGraph CreateGraph(List<string> files) {
            Graph = new SyntaxGraph();

            for (var i = 0; i < files.Count; i++) {
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
        public List<RecognitionResult> Run(List<DesignPattern> patterns) {
            var results = new List<RecognitionResult>();
            var j = 0;

            var entities = Graph.GetAll();
            foreach (var node in entities.Values) {
                j++;
                ProgressUpdate((int)(j / (float)entities.Count * 50f + 50), "Scanning class: " + node.GetName());
                results.AddRange(
                    from pattern in patterns
                    select new RecognitionResult {
                        Result = pattern.Recognizer.Recognize(node),
                        EntityNode = node,
                        FilePath = node.GetRoot().GetSource(),
                        Pattern = pattern
                    }
                );
            }

            return results;
        }

        private void ProgressUpdate(int percentage, string status) {
            OnProgressUpdate?.Invoke(this, new RecognizerProgress { CurrentPercentage = percentage, Status = status });
        }
    }
}
