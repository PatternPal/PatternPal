using System.Collections.Generic;
using System.IO;

namespace IDesign.Core
{
    public class ReadFiles
    {
        public List<string> Files = new List<string>();

        /// <summary>
        ///     Function that makes a string of a file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>
        ///     Returns a string of the content of a file
        /// </returns>
        public string MakeStringFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return "Path does not contains file!";
            return File.ReadAllText(filePath);
        }

        /// <summary>
        ///     Function that expects a directory path and adds all files equal to .cs files to a list
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns>
        ///     Returns a list of strings with file paths
        /// </returns>
        public List<string> GetFilesFromDirectory(string directoryPath)
        {
            var files = Directory.EnumerateFiles(directoryPath, "*.cs");
            if (files != null)
                foreach (var file in files)
                    Files.Add(file);
            return Files;
        }
    }
}