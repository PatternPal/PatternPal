using PatternPal.Core.Checks;
using PatternPal.Core;
using PatternPal.SyntaxTree;
using static PatternPal.Core.RecognizerRunner;

namespace PatternPal.Tests.Core.RecognizerRunnerTests;

[TestFixture]
internal class TestRecognizer_RunnerTests
{
    [Test]
    [TestCase("PruneTestClass1.cs", "lalal", true)]
    public void Knockout_Pruning(
        string filename,
        string node,
        bool shouldBePruned
        )
    {
        //Create testrecognizer
        string code = FileUtils.FileToString("PruneTests\\" + filename);
        string nameSpaceNode = "PatternPal.Tests.TestClasses.PruneTests";

        SyntaxGraph graph = new();
        graph.AddFile(code, code);
        graph.CreateGraph();

        TestRecognizer recognizer = new();
        ICheck rootCheck = recognizer.CreateRootCheck();

        IRecognizerContext ctx = new RecognizerContext
        {
            Graph = graph,
            CurrentEntity = null!,
            ParentCheck = rootCheck,
        };

        NodeCheckResult rootResult = (NodeCheckResult)rootCheck.Check(
            ctx,
            new RootNode());

        SortCheckResults(rootResult);

        // Filter the results.
        Dictionary<INode, List<ICheckResult>> resultsByNode = new();
        FilterResults(
            resultsByNode,
            rootResult);


        Assert.AreEqual(shouldBePruned, true);

        //Test if name of node is in result
        //Assert shouldBePruned
    }
}
