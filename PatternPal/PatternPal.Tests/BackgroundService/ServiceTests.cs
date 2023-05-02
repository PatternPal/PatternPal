#region

using System.Diagnostics;
using System.IO;

using Grpc.Core;
using Grpc.Net.Client;

using NUnit.Framework;

using PatternPal.Protos;

#endregion

namespace PatternPal.Tests.BackgroundService;

[TestFixture]
public class Tests
{
    private RecognizerService.RecognizerServiceClient _client;
    private GrpcChannel _channel;
    private static Process _backgroundService;
    private string _patternPalDirectory;

    [SetUp]
    public void Setup()
    {
        if (_backgroundService is not null
            && _backgroundService.StartTime != DateTime.MinValue)
        {
            return;
        }

        _patternPalDirectory = Path.Combine(
            Directory.GetCurrentDirectory(),
            "..",
            "..",
            "..",
            "..",
            "PatternPal",
            "bin",
            "Debug",
            "net7.0");
        string processPath = Path.Combine(
            _patternPalDirectory,
            "PatternPal.exe");

        if (!Path.Exists(processPath))
        {
            throw new ArgumentException("PatternPal exe couldn't be found");
        }

        // Start the process
        ProcessStartInfo processInfo = new()
                                       {
                                           FileName = processPath,
                                           WorkingDirectory = _patternPalDirectory
                                       };
        _backgroundService = Process.Start(processInfo);

        // Create a gRPC channel once because it is an expensive operation
        _channel = GrpcChannel.ForAddress("http://127.0.0.1:5001");
        // Make a recognizer service clients
        _client = new RecognizerService.RecognizerServiceClient(_channel);
    }

    [Test]
    public void DidBackgroundServiceStart()
    {
        Assert.IsTrue(_backgroundService.StartTime != DateTime.MinValue);
    }

    [Test]
    [Ignore("This doesn't work in CI")]
    // "Good" implemented design patterns
    [TestCase("SingleTonTestCase1.cs")]
    public Task ReceiveReponseFromService(
        string filename)
    {
        RecognizeRequest request = new RecognizeRequest
                                   {
                                       File = Directory.GetCurrentDirectory() + "\\TestClasses\\Singleton\\" + filename
                                   };

        request.Recognizers.Add(Recognizer.Singleton);
        request.ShowAllResults = true;

        IAsyncStreamReader< RecognizeResponse > responseStream = _client.Recognize(request).ResponseStream;

        IList< RecognizeResult > results = new List< RecognizeResult >();

        while (responseStream.MoveNext().Result)
        {
            results.Add(responseStream.Current.Result);
        }

        return Verifier.Verify(results);
    }

    /// <summary>
    /// Test if we receive results if "show all" is selected and wrong pattern is provided to a recognizer.
    /// </summary>
    /// <param name="filename"></param>
    [Test]
    [Ignore("This doesn't work in CI")]
    [TestCase("SingleTonTestCase10.cs")] // Offer the wrong pattern for the selected recognizer
    public void ReceiveResultsWithShowAll(
        string filename)
    {
        // Arrange
        RecognizeRequest request = new RecognizeRequest
                                   {
                                       File = Directory.GetCurrentDirectory() + "\\TestClasses\\Singleton\\" + filename
                                   };
        // Attach a bridge recognizer
        request.Recognizers.Add(Recognizer.Bridge);
        // Show all results
        request.ShowAllResults = true;

        // Act
        IAsyncStreamReader< RecognizeResponse > responseStream = _client.Recognize(request).ResponseStream;
        IList< RecognizeResult > results = new List< RecognizeResult >();
        while (responseStream.MoveNext().Result)
        {
            results.Add(responseStream.Current.Result);
        }

        // Assert
        // At least one result because the request was to show all results
        Assert.IsTrue(results.Count > 0);
    }

    /// <summary>
    /// Test if we receive no results if "show all" is not selected and wrong pattern is provided to a recognizer.
    /// </summary>
    [Test]
    [Ignore("This doesn't work in CI")]
    [TestCase("SingleTonTestCase10.cs")] // Offer the wrong pattern for the selected recognizer
    public void ReceiveNoResultsWithoutShowAll(
        string filename)
    {
        RecognizeRequest request = new RecognizeRequest
                                   {
                                       File = Directory.GetCurrentDirectory() + "\\TestClasses\\Singleton\\" + filename
                                   };

        // Attach a bridge recognizer
        request.Recognizers.Add(Recognizer.Bridge);
        // Do not show all results
        request.ShowAllResults = false;

        IAsyncStreamReader< RecognizeResponse > responseStream = _client.Recognize(request).ResponseStream;

        IList< RecognizeResult > results = new List< RecognizeResult >();

        while (responseStream.MoveNext().Result)
        {
            results.Add(responseStream.Current.Result);
        }

        // No results because the request was to not show bad results
        Assert.IsTrue(results.Count == 0);
        //return Verifier.Verify(results);
    }

    [Test]
    public void WasBackgroundServiceKilled()
    {
        // Act
        _backgroundService.Kill(true);

        // Assert
        Assert.IsTrue(_backgroundService is not null && _backgroundService.HasExited);
    }
}
