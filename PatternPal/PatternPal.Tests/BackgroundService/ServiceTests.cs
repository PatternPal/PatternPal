#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Grpc.Core;
using Grpc.Net.Client;

using NUnit.Framework;

using PatternPal.Protos;
using PatternPal.Tests.Utils;

using VerifyNUnit;

#endregion

namespace PatternPal.Tests.BackgroundService;

[TestFixture]
public class Tests
{
    private RecognizerService.RecognizerServiceClient _client;
    private GrpcChannel _channel;
    private static Process _backgroundService;

    [SetUp]
    public void Setup()
    {
        string processDirectory = Path.Combine(
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
            processDirectory,
            "PatternPal.exe");

        if (!Path.Exists(processPath))
        {
            throw new ArgumentException("PatternPal exe couldn't be found");
        }

        // Start the process
        ProcessStartInfo processInfo = new()
                                       {
                                           FileName = processPath,
                                           WorkingDirectory = processDirectory
                                       };
        _backgroundService = Process.Start(processInfo);

        // Create a gRPC channel once because it is an expensive operation
        _channel = GrpcChannel.ForAddress("http://localhost:5001");
        // Make a recognizer service clients
        _client = new RecognizerService.RecognizerServiceClient(_channel);
    }

    [Test]
    public void DidBackgroundServiceStart()
    {
        // Arrange
        Process[ ] process = Process.GetProcessesByName("PatternPal");

        // Assert
        Assert.IsTrue(process.Length > 0);
    }

    [Test]
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
        _backgroundService.Kill(true);
        // Arrange
        Process[ ] process = Process.GetProcessesByName("patternpal");

        // Act
        foreach (Process processItem in process)
        {
            processItem.Kill();
        }

        //Assert
        bool allExited = process.Skip(1).All(p => p.HasExited);
        Assert.IsTrue(allExited);
    }
}
