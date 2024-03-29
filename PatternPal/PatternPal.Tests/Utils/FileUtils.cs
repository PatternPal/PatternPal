﻿#region

using System.IO;

#endregion

namespace PatternPal.Tests.Utils
{
    public static class FileUtils
    {
        /// <summary>
        ///     Returns a string of all the contents from the given file.
        /// </summary>
        /// <param name="fileName">Path of the file which needs to be read</param>
        /// <returns>The file content</returns>
        public static string FileToString(string fileName)
        {
            try
            {
                var streamReader = new StreamReader("TestClasses\\" + fileName);
                return streamReader.ReadToEnd();
            }
            catch (IOException)
            {
                throw new Exception("File not found. Make sure test files have 'Copy always' on");
            }
        }

        /// <summary>
        ///     returns a list of strings of the contents of all the files
        /// </summary>
        /// <param name="folderPath">Path of the folder in which all files need to be read</param>
        /// <returns>List with all file contents </returns>
        public static List<string> FilesToString(string folderPath)
        {
            List<string> filesContents = new();
            foreach (string file in Directory.EnumerateFiles("TestClasses\\" + folderPath, "*.cs"))
            {
                filesContents.Add(File.ReadAllText(file));
            }

            return filesContents;
        }
    }
}
