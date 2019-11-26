using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IDesign.Tests.Utils
{
    public static class FileUtils
    {
        public static string FileToString(string fileName)
        {
            var streamReader = new StreamReader("TestClasses\\" + fileName);
                return streamReader.ReadToEnd();
        }
    }
}
