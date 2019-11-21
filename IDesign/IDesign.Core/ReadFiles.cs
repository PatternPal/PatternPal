using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IDesign.Core
{
    public class ReadFiles
    {
        public List<string> Files = new List<string>();

        public string MakeStringFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return "Path does not contains file!";
            }

            return File.ReadAllText(filePath);
        }

        public void GetFilesFromDirectory(string directoryPath)
        {
            var files = Directory.EnumerateFiles(directoryPath, "*.cs");
            foreach (var file in files)
            {
                Files.Add(file);
            }
        }
    }
}
