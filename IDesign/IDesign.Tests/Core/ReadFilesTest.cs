using IDesign.Core;
using NUnit.Framework;
using System.Collections.Generic;

namespace IDesign.Tests.Core
{
    internal class ReadFilesTest
    {
        public string path = "../../../Core/TestClasses";
        private readonly ReadFiles readFiles = new ReadFiles();

        [Test]
        public void TestIfFilesListContainsRightFiles()
        {
            var expected = new List<string>();
            expected.Add(@"../../../Core/TestClasses\ITest.cs");
            expected.Add(@"../../../Core/TestClasses\TestClass1.cs");

            var actual = readFiles.GetFilesFromDirectory(path);
            Assert.AreEqual(expected, actual);
        }
    }
}