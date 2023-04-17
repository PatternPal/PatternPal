using System.Collections.Generic;

using NUnit.Framework;

namespace PatternPal.Tests.Core
{
    internal class FileManagerTest
    {
        private readonly FileManager readFiles = new FileManager();
        public string path = "../../../Core/TestClasses";

        [Test]
        public void TestIfFilesListContainsRightFiles()
        {
            var expected = new List<string>
            {
                @"../../../Core/TestClasses\ITest.cs", @"../../../Core/TestClasses\TestClass1.cs"
            };

            var actual = readFiles.GetAllCSharpFilesFromDirectory(path);
            Assert.AreEqual(expected, actual);
        }
    }
}
