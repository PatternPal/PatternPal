using System;
using System.Collections.Generic;
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

        public static List<string> FilesToString(string folderPath)
        {
            var filesContents = new List<string>();
            foreach (string file in Directory.EnumerateFiles("TestClasses\\" + folderPath, "*.cs"))
            {
                filesContents.Add(File.ReadAllText(file));
            }
            return filesContents;

        }
    }
}