using System.IO;

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
