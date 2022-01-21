using System;
using PatternPal.CommonResources;
using PatternPal.Recognizers.Abstractions;
using PatternPal.Recognizers.Recognizers;
using PatternPal.Tests.Utils;
using NUnit.Framework;
using SyntaxTree;

namespace PatternPal.Tests.Recognizers
{
    internal class StrategyRecognizerTest
    {
        [TestCase("StateTest1", "Doneness", 0, 79)]
        [TestCase("StateTest2", "State", 0, 79)]
        [TestCase("StateTest3", "State", 0, 79)]
        [TestCase("StrategyTest1", "CookStrategy", 80, 100)]
        [TestCase("StrategyTest2", "IStrategy", 80, 100)]
        [TestCase("StrategyTest3", "Strategy", 80, 100)]
        [TestCase("StrategyFactoryMethodTest1", "IStrategy", 80, 100)]
        [TestCase("StateFactoryMethodTest1", "IState", 0, 79)]
        public void StrategyRecognizer_Returns_Correct_Score(
            string directory,
            string filename,
            int minScore,
            int maxScore
        )
        {
            var strategy = new StrategyRecognizer();
            var filesAsString = FileUtils.FilesToString($"{directory}\\");
            var nameSpaceName = $"PatternPal.Tests.TestClasses.{directory}";


            var graph = new SyntaxGraph();
            var i = 0;
            foreach (var s in filesAsString)
            {
                graph.AddFile(s, i++.ToString());
            }

            graph.CreateGraph();

            var entityNodes = graph.GetAll();
            var result = strategy.Recognize(entityNodes[nameSpaceName + "." + filename]);

            foreach (var checkResult in result.GetResults())
            {
                PrintResult(checkResult, 1);
            }

            Assert.That(result.GetScore(), Is.InRange(minScore, maxScore));
        }

        public static void PrintResult(ICheckResult result, int depth)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            var symbol = "X";

            switch (result.GetFeedbackType())
            {
                case FeedbackType.SemiCorrect:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    symbol = "-";
                    break;
                case FeedbackType.Correct:
                    Console.ForegroundColor = ConsoleColor.Green;
                    symbol = "✓";
                    break;
            }

            Console.WriteLine(
                new string('\t', depth) + symbol +
                $"{ResourceUtils.ResultToString(result)} | {result.GetScore()}p / {result.GetTotalChecks()}p"
            );

            foreach (var child in result.GetChildFeedback())
            {
                PrintResult(child, depth + 1);
            }
        }
    }
}
