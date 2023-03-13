#region

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

using PatternPal.Core.Models;
using PatternPal.Core.Resources;
using PatternPal.Protos;
using PatternPal.Recognizers;
using PatternPal.Recognizers.Recognizers;

using SyntaxTree;

using static PatternPal.Core.Resources.DesignPatternNameResources;

#endregion

namespace PatternPal.Core
{
    public class RecognizerRunner
    {
        public static readonly ImmutableList< DesignPattern > DesignPatterns = ImmutableList.Create(
            new DesignPattern(
                Singleton,
                new SingletonRecognizer(),
                WikiPageResources.Singleton),
            new DesignPattern(
                FactoryMethod,
                new FactoryMethodRecognizer(),
                WikiPageResources.FactoryMethod),
            new DesignPattern(
                Decorator,
                new DecoratorRecognizer(),
                WikiPageResources.Decorator),
            new DesignPattern(
                State,
                new StateRecognizer(),
                WikiPageResources.State),
            new DesignPattern(
                Strategy,
                new StrategyRecognizer(),
                WikiPageResources.Strategy),
            new DesignPattern(
                Adapter,
                new AdapterRecognizer(),
                WikiPageResources.Adapter),
            new DesignPattern(
                Observer,
                new ObserverRecognizer(),
                WikiPageResources.Observer),
            new DesignPattern(
                Bridge,
                new BridgeRecognizer(),
                WikiPageResources.Bridge)
        );

        private static readonly DesignPattern[ ] s_DesignPatterns;

        static RecognizerRunner()
        {
            // TODO: Assert that these patterns are added in the same order as the definitions in the enum.
            s_DesignPatterns = new[ ]
                               {
                                   new DesignPattern(
                                       Adapter,
                                       new AdapterRecognizer(),
                                       WikiPageResources.Adapter),
                                   new DesignPattern(
                                       Bridge,
                                       new BridgeRecognizer(),
                                       WikiPageResources.Bridge),
                                   new DesignPattern(
                                       Decorator,
                                       new DecoratorRecognizer(),
                                       WikiPageResources.Decorator),
                                   new DesignPattern(
                                       FactoryMethod,
                                       new FactoryMethodRecognizer(),
                                       WikiPageResources.FactoryMethod),
                                   new DesignPattern(
                                       Observer,
                                       new ObserverRecognizer(),
                                       WikiPageResources.Observer),
                                   new DesignPattern(
                                       Singleton,
                                       new SingletonRecognizer(),
                                       WikiPageResources.Singleton),
                                   new DesignPattern(
                                       State,
                                       new StateRecognizer(),
                                       WikiPageResources.State),
                                   new DesignPattern(
                                       Strategy,
                                       new StrategyRecognizer(),
                                       WikiPageResources.Strategy),
                               };
        }

        public static DesignPattern GetDesignPattern(
            Recognizer recognizer) => s_DesignPatterns[ ((int)recognizer) - 1 ];

        public SyntaxGraph Graph;

        public event EventHandler< RecognizerProgress > OnProgressUpdate;

        public SyntaxGraph CreateGraph(
            List< string > files)
        {
            Graph = new SyntaxGraph();

            for (var i = 0;
                 i < files.Count;
                 i++)
            {
                var content = FileManager.MakeStringFromFile(files[ i ]);
                Graph.AddFile(
                    content,
                    files[ i ]);
                ProgressUpdate(
                    (int)(i / (float)files.Count * 50f),
                    "Reading file: " + Path.GetFileName(files[ i ]));
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
        public List< RecognitionResult > Run(
            List< DesignPattern > patterns)
        {
            var results = new List< RecognitionResult >();
            var j = 0;

            if (Graph == null)
                return results;

            var entities = Graph.GetAll();

            if (entities == null)
                return results;
            foreach (var node in entities.Values)
            {
                j++;
                ProgressUpdate(
                    (int)((j / (float)entities.Count * 50f) + 50),
                    "Scanning class: " + node.GetName());
                results.AddRange(
                    from pattern in patterns
                    select new RecognitionResult
                           {
                               Result = pattern.Recognizer.Recognize(node), EntityNode = node, FilePath = node.GetRoot().GetSource(), Pattern = pattern
                           }
                );
            }

            return results;
        }

        private void ProgressUpdate(
            int percentage,
            string status)
        {
            OnProgressUpdate?.Invoke(
                this,
                new RecognizerProgress
                {
                    CurrentPercentage = percentage, Status = status
                });
        }
    }
}
