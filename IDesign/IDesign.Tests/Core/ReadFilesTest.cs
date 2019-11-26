using IDesign.Core;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDesign.Tests.Core
{
    class ReadFilesTest
    {
        public string path = "../../../Core/TestClasses";
        FileManager readFiles = new FileManager();

        [Test]
        public void TestIfFilesListContainsRightFiles()
        {
            List<string> expected = new List<string>();
            expected.Add(@"../../../Core/TestClasses\ITest.cs");
            expected.Add(@"../../../Core/TestClasses\TestClass1.cs");

            List<string> actual = readFiles.GetAllCsFilesFromDirectory(path);
            Assert.AreEqual(expected, actual);
        }
    }
}
