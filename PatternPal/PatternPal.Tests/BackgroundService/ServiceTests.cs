#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using NUnit.Framework;
using PatternPal.Protos;
using VerifyNUnit;

#endregion

namespace PatternPal.Service_Tests
{
    public class Tests
    {
        private RecognizerService.RecognizerServiceClient _client;
        private GrpcChannel channel;

        [SetUp]
        public void Setup()
        {
            // Create a gRPC channel once because it is an expensive operation
            channel = GrpcChannel.ForAddress(
                "http://localhost:5000",
                new GrpcChannelOptions
                {
                    HttpHandler = new GrpcWebHandler(new HttpClientHandler()),
                });
            // Make a recognizer service client
            _client = new RecognizerService.RecognizerServiceClient(channel);
        }

        /// <summary>
        /// Test if the result from the service singleton recognizer is always the same
        /// </summary>
        [Test]
        [TestCase("SingleTonTestCase1.cs")]
        //[TestCase("SingleTonTestCase7.cs", 0, 79)]
        //[TestCase("SingleTonTestCase8.cs", 0, 79)]
        //[TestCase("SingleTonTestCase9.cs", 0, 79)]
        //[TestCase("SingleTonTestCase10.cs", 0, 79)]
        public Task ReceiveBadScoreSingleton(string filename)
        {

            RecognizeRequest request = new RecognizeRequest { File = Directory.GetCurrentDirectory() + "\\TestClasses\\Singleton\\" + filename };

            request.Recognizers.Add(Recognizer.Singleton);

            IAsyncStreamReader<RecognizeResponse> responseStream = _client.Recognize(request).ResponseStream;
            
            IList< RecognizeResult > results = new List< RecognizeResult >();

            while ( responseStream.MoveNext().Result )
            {
                results.Add(responseStream.Current.Result);
            }

            //Snapshot.Match(results);
            return Verifier.Verify(results);

            //TODO snapshot testing .json 

        }

        //[Test]
        //[TestCase("SingleTonTestCase1.cs")]
        //[TestCase("SingleTonTestCase2.cs")]
        //[TestCase("SingleTonTestCase4.cs")]
        //[TestCase("SingleTonTestCase5.cs")]
        //[TestCase("SingleTonTestCase6.cs")]
        //public void ReceiveGoodScoreSingleton(string filename)
        //{
        //    Assert.Pass();
        //}

        [Test]
        public void DidBackgroundServiceStart()
        {
            bool isRunning = false;
            Process[] processName = Process.GetProcessesByName("PatternPal");
            if (processName.Length == 0)
            {
                isRunning = true;
            }

            Assert.IsTrue(isRunning);
        }

        [Test]
        public void WasBackgroundServiceKilled()
        {
            bool success = false;

            try
            {
                Process[] processName = Process.GetProcessesByName("PatternPal");
            }
            catch (InvalidOperationException)
            {
                success = true;
            }


            Assert.IsTrue(success);
        }
    }
}
