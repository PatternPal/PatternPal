using System.Collections.Generic;
using IDesign.Core;
using NUnit.Framework;

namespace IDesign.Tests.Core
{
    internal class FileManagerTest
    {
        private readonly FileManager readFiles = new FileManager();
        public string path = "../../../Core/TestClasses";

        [Test]
        public void TestIfFilesListContainsRightFiles()
        {
            var expected = new List<string>();
            expected.Add(@"../../../Core/TestClasses\ITest.cs");
            expected.Add(@"../../../Core/TestClasses\TestClass1.cs");

            var actual = readFiles.GetAllCsFilesFromDirectory(path);
            Assert.AreEqual(expected, actual);
        }
    }
}