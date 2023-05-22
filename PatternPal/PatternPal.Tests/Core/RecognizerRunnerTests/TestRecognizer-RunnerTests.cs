using PatternPal.Core.Checks;
using PatternPal.Core;
using PatternPal.SyntaxTree;
using static PatternPal.Core.RecognizerRunner;

namespace PatternPal.Tests.Core.RecognizerRunnerTests;

[TestFixture]
internal class TestRecognizer_RunnerTests
{
    [Test]
    [TestCase("PruneTestClass1.cs")]
    [TestCase("PruneTestClass2.cs")]
    public Task Knockout_Pruning(string filename)
    {
        //Create testrecognizer
        string code = FileUtils.FileToString("PruneTests\\" + filename);

        SyntaxGraph graph = new();
        graph.AddFile(code, code);
        graph.CreateGraph();

        IRecognizer recognizer = new TestRecognizer();
        ICheck rootCheck = recognizer.CreateRootCheck();

        IRecognizerContext ctx = RecognizerContext4Tests.Create(graph, rootCheck);

        NodeCheckResult rootResult = (NodeCheckResult)rootCheck.Check(
            ctx,
            new RootNode4Tests());

        SortCheckResults(rootResult);

        // Filter the results.
        Dictionary<INode, List<ICheckResult>> resultsByNode = new();
        FilterResults(
            resultsByNode,
            rootResult);

        return Verifier.Verify(rootResult);
    }
}
