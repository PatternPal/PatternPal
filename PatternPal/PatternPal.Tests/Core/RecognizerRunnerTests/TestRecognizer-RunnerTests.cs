using PatternPal.SyntaxTree;

namespace PatternPal.Tests.Core.RecognizerRunnerTests;

[TestFixture]
internal class TestRecognizer_RunnerTests
{
    [Test]
    public void Knockout_Pruning(
        string filename,
        string node,
        bool shouldBePruned
        )
    {
        //Create testrecognizer
        string code = FileUtils.FileToString("Relation\\MethodsAndEntities\\" + filename);
        string nameSpaceNode = "PatternPal.Tests.TestClasses.Relation.MethodsAndEntities";

        SyntaxGraph graph = new();
        graph.AddFile(code, code);
        graph.CreateGraph();


        //Test if name of node is in result
        //Assert shouldBePruned
    }
}
