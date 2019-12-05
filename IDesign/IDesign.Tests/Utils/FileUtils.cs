using System;
using System.IO;

namespace IDesign.Tests.Utils
{
    public static class FileUtils
    {
        public static string FileToString(string fileName)
        {
            try
            {
                var streamReader = new StreamReader("TestClasses\\" + fileName);
                return streamReader.ReadToEnd();
            }
            catch (IOException)
            {
                throw new Exception("File not found. Make sure testfiles have 'Copy always' on");
            }

        }
    }
}