using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IDesign.Core
{
    public class FileManager
    {
        /// <summary>
        ///     Function that makes a string of a file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>
        ///     Returns a string of the content of a file
        /// </returns>
        public static string MakeStringFromFile(string filePath)
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
        public List<string> GetAllCsFilesFromDirectory(string directoryPath)
        {
            return Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories).ToList();
        }
    }
}