#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using NUnit.Framework;
using PatternPal.Protos;
using PatternPal.Tests.Utils;
using VerifyNUnit;

#endregion

namespace PatternPal.Service_Tests
{
    public class Tests
    {
        private RecognizerService.RecognizerServiceClient _client;
        private GrpcChannel channel;
        private static Process _backgroundService;

        [SetUp]
        public void Setup()
        {
            string extensionDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo solutionPath = FileUtils.ObtainSolutionFolder(extensionDirectory);

            string processPath = Path.Combine(
                solutionPath.FullName,
                "PatternPal.Extension",
                "bin",
                "Debug",
                "PatternPal",
                "PatternPal.exe");

            _backgroundService = Process.Start(
                new ProcessStartInfo(processPath)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                });

            // Create a gRPC channel once because it is an expensive operation
            channel = GrpcChannel.ForAddress("http://localhost:5001");
            // Make a recognizer service client
            _client = new RecognizerService.RecognizerServiceClient(channel);
        }

        [Test]
        public void DidBackgroundServiceStart()
        {
            Process[] process = Process.GetProcessesByName("PatternPal");
            Assert.IsTrue(process.Length > 0);
        }

        [Test]
        // "Good" implemented design patterns
        [TestCase("SingleTonTestCase1.cs")]
        //[TestCase("SingleTonTestCase2.cs")]
        [TestCase("SingleTonTestCase4.cs")]
        [TestCase("SingleTonTestCase5.cs")]
        //[TestCase("SingleTonTestCase6.cs")]
        public Task ReceiveReponseFromService(
            string filename)
        {
            RecognizeRequest request = new RecognizeRequest
            {
                File = Directory.GetCurrentDirectory() + "\\TestClasses\\Singleton\\" + filename
            };

            request.Recognizers.Add(Recognizer.Singleton);
            request.ShowAllResults = true;

            IAsyncStreamReader<RecognizeResponse> responseStream = _client.Recognize(request).ResponseStream;

            IList<RecognizeResult> results = new List<RecognizeResult>();

            while (responseStream.MoveNext().Result)
            {
                results.Add(responseStream.Current.Result);
            }

            return Verifier.Verify(results);
        }

        /// <summary>
        /// Test if we receive no results if "show all" is not pressed and wrong pattern is provided to a recognizer.
        /// </summary>
        [Test]
        [TestCase("SingleTonTestCase7.cs")] // Offer the wrong pattern for the selected recognizer
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

        /// <summary>
        /// Test if we receive results if "show all" is selected and wrong pattern is provided to a recognizer.
        /// </summary>
        /// <param name="filename"></param>
        [Test]
        [TestCase("SingleTonTestCase10.cs")] // Offer the wrong pattern for the selected recognizer
        public void ReceiveResultsWithShowAll(
            string filename)
        {
            RecognizeRequest request = new RecognizeRequest
            {
                File = Directory.GetCurrentDirectory() + "\\TestClasses\\Singleton\\" + filename
            };

            // Attach a bridge recognizer
            request.Recognizers.Add(Recognizer.Bridge);
            // Show all results
            request.ShowAllResults = true;

            IAsyncStreamReader<RecognizeResponse> responseStream = _client.Recognize(request).ResponseStream;

            IList<RecognizeResult> results = new List<RecognizeResult>();

            while (responseStream.MoveNext().Result)
            {
                results.Add(responseStream.Current.Result);
            }

            // At least one result because the request was to show all results
            Assert.IsTrue(results.Count > 0);
        }

        [Test]
        public void WasBackgroundServiceKilled()
        {
            _backgroundService.Kill();
            Process[] process = Process.GetProcessesByName("patternpal");
            Assert.IsTrue(process.Length == 0);
        }
    }
}
