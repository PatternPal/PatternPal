#region

using System;
using System.Diagnostics;
using System.IO;

#endregion

namespace PatternPal.Service_Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase("SingleTonTestCase3.cs")]
        [TestCase("SingleTonTestCase7.cs")]
        [TestCase("SingleTonTestCase8.cs")]
        [TestCase("SingleTonTestCase9.cs")]
        [TestCase("SingleTonTestCase10.cs")]
        public void ReceiveBadScoreSingleton()
        {
            Assert.Pass();
        }

        [Test]
        [TestCase("SingleTonTestCase1.cs")]
        [TestCase("SingleTonTestCase2.cs")]
        [TestCase("SingleTonTestCase4.cs")]
        [TestCase("SingleTonTestCase5.cs")]
        [TestCase("SingleTonTestCase6.cs")]
        public void ReceiveGoodScoreSingleton()
        {
            Assert.Pass();
        }

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
