#region

using System;
using System.Collections.Generic;

using PatternPal.Core.Models;
using PatternPal.Core.Resources;
using PatternPal.Protos;
using PatternPal.Recognizers;
using PatternPal.Recognizers.Recognizers;

using SyntaxTree;
using SyntaxTree.Abstractions.Entities;

using static PatternPal.Core.Resources.DesignPatternNameResources;

#endregion

namespace PatternPal.Core
{
    /// <summary>
    /// This class is the driver which handles running the recognizers.
    /// </summary>
    public class RecognizerRunner
    {
        /// <summary>
        /// Currently supported design patterns.
        /// </summary>
        public static readonly DesignPattern[ ] DesignPatterns;

        static RecognizerRunner()
        {
            // TODO: Assert that these patterns are added in the same order as the definitions in the enum.
            DesignPatterns = new[ ]
                             {
                                 new DesignPattern(
                                     Adapter,
                                     new AdapterRecognizer(),
                                     WikiPageResources.Adapter,
                                     Recognizer.Adapter),
                                 new DesignPattern(
                                     Bridge,
                                     new BridgeRecognizer(),
                                     WikiPageResources.Bridge,
                                     Recognizer.Bridge),
                                 new DesignPattern(
                                     Decorator,
                                     new DecoratorRecognizer(),
                                     WikiPageResources.Decorator,
                                     Recognizer.Decorator),
                                 new DesignPattern(
                                     FactoryMethod,
                                     new FactoryMethodRecognizer(),
                                     WikiPageResources.FactoryMethod,
                                     Recognizer.FactoryMethod),
                                 new DesignPattern(
                                     Observer,
                                     new ObserverRecognizer(),
                                     WikiPageResources.Observer,
                                     Recognizer.Observer),
                                 new DesignPattern(
                                     Singleton,
                                     new SingletonRecognizer(),
                                     WikiPageResources.Singleton,
                                     Recognizer.Singleton),
                                 new DesignPattern(
                                     State,
                                     new StateRecognizer(),
                                     WikiPageResources.State,
                                     Recognizer.State),
                                 new DesignPattern(
                                     Strategy,
                                     new StrategyRecognizer(),
                                     WikiPageResources.Strategy,
                                     Recognizer.Strategy),
                             };
        }

        private readonly IList< DesignPattern > _patterns;
        private SyntaxGraph _graph;

        /// <summary>
        /// Create a new recognizer runner instance.
        /// </summary>
        /// <param name="files">The files to run the recognizers on.</param>
        /// <param name="recognizers">The recognizers to run.</param>
        public RecognizerRunner(
            IEnumerable< string > files,
            IEnumerable< Recognizer > recognizers)
        {
            CreateGraph(files);

            // Get the design patterns which correspond to the given recognizers.
            _patterns = new List< DesignPattern >();
            foreach (Recognizer recognizer in recognizers)
            {
                // `Recognizer.Unknown` is the default value of the `Recognizer` enum, as required
                // by the Protocol Buffer spec. This value should never be used.
                if (recognizer == Recognizer.Unknown)
                {
                    continue;
                }

                _patterns.Add(DesignPatterns[ ((int)recognizer) - 1 ]);
            }
        }

        /// <summary>
        /// Create a new recognizer runner instance.
        /// </summary>
        /// <param name="files">The files to run the recognizers on.</param>
        /// <param name="patterns">The design patterns for which to run the recognizers.</param>
        public RecognizerRunner(
            IEnumerable< string > files,
            IList< DesignPattern > patterns)
        {
            CreateGraph(files);
            _patterns = patterns;
        }

        /// <summary>
        /// Creates a <see cref="SyntaxGraph"/> from the given files.
        /// </summary>
        /// <param name="files">The files from which to create a <see cref="SyntaxGraph"/></param>
        private void CreateGraph(
            IEnumerable< string > files)
        {
            _graph = new SyntaxGraph();
            foreach (string file in files)
            {
                string content = FileManager.MakeStringFromFile(file);
                _graph.AddFile(
                    content,
                    file);
            }
            _graph.CreateGraph();
        }

        public event EventHandler< RecognizerProgress > OnProgressUpdate;

        /// <summary>
        /// Run the recognizers.
        /// </summary>
        /// <returns>A list of <see cref="RecognitionResult"/>, one per given design pattern.</returns>
        public IList< RecognitionResult > Run()
        {
            // If the graph is empty, we don't have to do any work.
            if (_graph.IsEmpty)
            {
                return new List< RecognitionResult >();
            }

            int nodeIdx = 0;
            Dictionary< string, IEntity > entities = _graph.GetAll();
            IList< RecognitionResult > results = new List< RecognitionResult >();
            foreach (IEntity node in entities.Values)
            {
                ReportProgress(
                    (int)((++nodeIdx / (float)entities.Count * 50f) + 50),
                    "Scanning class: " + node.GetName());

                // Run the recognizers.
                foreach (DesignPattern pattern in _patterns)
                {
                    results.Add(
                        new RecognitionResult
                        {
                            Result = pattern.Recognizer.Recognize(node),
                            EntityNode = node,
                            FilePath = node.GetRoot().GetSource(),
                            Pattern = pattern
                        });
                }
            }

            return results;
        }

        /// <summary>
        /// Report a progress update.
        /// </summary>
        /// <param name="percentage">The current progress as a percentage.</param>
        /// <param name="status">A status message associated with the current progress.</param>
        private void ReportProgress(
            int percentage,
            string status)
        {
            OnProgressUpdate?.Invoke(
                this,
                new RecognizerProgress
                {
                    CurrentPercentage = percentage,
                    Status = status
                });
        }
    }
}
