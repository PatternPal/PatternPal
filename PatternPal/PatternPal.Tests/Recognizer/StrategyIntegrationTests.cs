namespace PatternPal.Tests.Core.RecognizerRunnerTests;

[TestFixture]
internal class StrategyIntegrationTests
{

    [Test]
    [TestCase("StrategyTestCase01.cs")]
    [TestCase("StrategyTestCase02.cs")]
    [TestCase("StrategyTestCase03.cs")]
    [TestCase("StrategyTestCase04.cs")]
    [TestCase("StrategyTestCase05.cs")]
    [TestCase("StrategyTestCase06.cs")]
    [TestCase("StrategyTestCase07.cs")]
    [TestCase("StrategyTestCase08.cs")]
    [TestCase("StrategyTestCase09.cs")]
    [TestCase("StrategyTestCase10.cs")]
    [TestCase("StrategyTestCase11.cs")]
    [TestCase("StrategyTestCase12.cs")]
    [TestCase("StrategyTestCase13.cs")]
    [TestCase("StrategyTestCase14.cs")]
    [TestCase("StrategyTestCase15.cs")]
    [TestCase("StrategyTestCase16.cs")]
    [TestCase("StrategyTestCase17.cs")]
    [TestCase("StrategyTestCase18.cs")]
    public Task StrategyIntegrationTest(string file)
    {
        IRecognizer strategyRecognizer = new StrategyRecognizer();
        List<string> files = new()
        {
            @"New_TestCasesRecognizers/Strategy/" + file
        };
        List<IRecognizer> recognizers = new()
        {
            strategyRecognizer
        };

        RecognizerRunner runner = new RecognizerRunner(files, recognizers);

        return Verifier.Verify(runner.Run());
    }
}
