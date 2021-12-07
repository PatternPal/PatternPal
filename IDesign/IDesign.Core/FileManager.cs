using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IDesign.Core
{
    public class FileManager
    {
        public static string MakeStringFromFile(string filePath)
        {
            return !File.Exists(filePath) ? "Path does not contains file!" : File.ReadAllText(filePath);
        }

        public List<string> GetAllCSharpFilesFromDirectory(string directoryPath)
        {
            return Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories).ToList();
        }
    }
}
