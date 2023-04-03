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

using VerifyNUnit;

#endregion

namespace PatternPal.Tests.BackgroundService
{
    [TestFixture]
    public class Tests
    {
        private RecognizerService.RecognizerServiceClient _client;
        private GrpcChannel _channel;
        private static Process _backgroundService;

        [SetUp]
        public void Setup()
        {
            // TODO start process and test
            //string backgroundServicePath = Path.Combine(
            //    Directory.GetCurrentDirectory(),
            //    "PatternPal",
            //    "PatternPal.exe");
            //_backgroundService = Process.Start(
            //    new ProcessStartInfo(backgroundServicePath)
            //    {
            //        CreateNoWindow = true,
            //        UseShellExecute = false,
            //    });

            // Create a gRPC channel once because it is an expensive operation
            _channel = GrpcChannel.ForAddress("http://localhost:5001");

            // Make a recognizer service client
            _client = new RecognizerService.RecognizerServiceClient(_channel);
        }

        /// <summary>
        /// Test if the result from the service singleton recognizer is none because no well implemented.
        /// </summary>
        [Test]
        [TestCase("SingleTonTestCase1.cs")]
        public Task ReceiveNoScoresBadSingleton(
            string filename)
        {
            RecognizeRequest request = new RecognizeRequest
                                       {
                                           File = Path.Combine(
                                               Directory.GetCurrentDirectory(),
                                               "\\TestClasses\\Singleton\\",
                                               filename)
                                       };

            request.Recognizers.Add(Recognizer.Singleton);

            IAsyncStreamReader< RecognizeResponse > responseStream = _client.Recognize(request).ResponseStream;

            IList< RecognizeResult > results = new List< RecognizeResult >();

            while (responseStream.MoveNext().Result)
            {
                results.Add(responseStream.Current.Result);
            }
            // TODO receive no scores when show all is toggled "OFF"
            return Verifier.Verify(results);

        }

        

        [Test]
        public void DidBackgroundServiceStart()
        {
            bool isRunning = false;
            Process[ ] processName = Process.GetProcessesByName("PatternPal");
            if (processName.Length == 0)
            {
                isRunning = true;
            }

            Assert.IsTrue(isRunning);
        }

        /// <summary>
        /// Test whether killing the process was successful.
        /// </summary>
        [OneTimeTearDown]
        public void CleanUp()
        {
            // TODO change into locally started patternpal process
            Process temp = new Process();
            temp.Kill();
            bool killed = false;

            // Check if process exists
            try
            {
                Process[] Processname = Process.GetProcessesByName("PatternPal");
            }
            catch (InvalidOperationException)
            {
                killed = true; // Process does not exist
            }

            Assert.IsTrue(killed);
        }
        
    }
}
