#region

using System.IO;

#endregion

namespace PatternPal.Core;

/// <summary>
/// Helper methods for working with files.
/// </summary>
public class FileManager
{
    /// <summary>
    /// Gets all the C# source files from <paramref name="directoryPath"/> recursively.
    /// </summary>
    /// <param name="directoryPath">The directory to search in.</param>
    /// <returns>A <see cref="List{T}"/> of file paths.</returns>
    public List< string > GetAllCSharpFilesFromDirectory(
        string directoryPath)
    {
        return Directory.GetFiles(
            directoryPath,
            "*.cs",
            SearchOption.AllDirectories).ToList();
    }
}
