#region
using ls = PatternPal.Services.LoggingService;
using System.IO;
using System.IO.Compression;
using Google.Protobuf;

#endregion

namespace PatternPal.Tests.LoggingService
{
    [TestFixture]
    internal class ZipDirectoryTests
    {
        private const string TEMPDIR = "zipTestTemp";

        /// <summary>
        /// Creates the temporary working directory for before each test; deletes
        /// it first if it already existed.
        /// </summary>
        [SetUp]
        public void Init()
        {
            if (Directory.Exists(TEMPDIR))
            {
                Directory.Delete(TEMPDIR, true);
            }

            Directory.CreateDirectory(TEMPDIR);
        }

        /// <summary>
        /// Zips the directory supplied at path (containing only *.cs-files) and checks if unzipping the result
        /// yields the same directory as supplied.
        /// </summary>
        /// <param name="path">The path to the resource directory, containing only *.cs-files</param>
        [Test]
        [TestCase("../../../LoggingService/Resources/OnlyCsFiles")]
        [TestCase("../../../TestClasses/StrategyFactoryMethodTest1")]
        [TestCase("../../../TestClasses/AdapterTest1")]
        public void Unzipped_ByteString_Matches_Unzipped_Directory_Only_Cs_Files(string path)
        {
            string fullPath = Path.GetFullPath(path);
            ByteString archive = ls.ZipDirectory(fullPath);
            Unzip(archive, TEMPDIR);

            Assert.IsTrue(CompareDirectories(fullPath, TEMPDIR));
        }

        /// <summary>
        /// Zips the directory supplied at path and checks if unzipping the result
        /// yields the same directory as the one supplied minus all non-*.cs-files.
        /// </summary>
        /// <param name="path">The path to the resource directory</param>
        /// <param name="expected">The path to a copy of the resource directory minus all non-*.cs-files</param>
        [Test]
        [TestCase("../../../LoggingService/Resources/MixedFiles1", "../../../LoggingService/Resources/MixedFiles1Exp")]
        [TestCase("../../../LoggingService/Resources/MixedFiles2", "../../../LoggingService/Resources/MixedFiles2Exp")]
        public void Unzipped_ByteString_Matches_Unzipped_Directory_Mixed_Files(string path, string expected)
        {
            string fullPath = Path.GetFullPath(path);
            ByteString archive = ls.ZipDirectory(fullPath);
            Unzip(archive, TEMPDIR);

            Assert.IsTrue(CompareDirectories(expected, TEMPDIR));
        }

        /// <summary>
        /// Zips the directory supplied at path and checks if unzipping the result
        /// yields the same directory as the one supplied minus all contents of subfolders named 'bin' and 'obj'.
        /// </summary>
        /// <param name="path">The path to the resource directory</param>
        /// <param name="expected">The path to a copy of the resource directory minus all build artifacts</param>
        [Test]
        [TestCase("../../../LoggingService/Resources/BinObj", "../../../LoggingService/Resources/BinObjExp")]
        public void Unzipped_ByteString_Excludes_Build_Artifacts(string path, string expected)
        {
            string fullPath = Path.GetFullPath(path);
            ByteString archive = ls.ZipDirectory(fullPath);
            Unzip(archive, TEMPDIR);

            Assert.IsTrue(CompareDirectories(expected, TEMPDIR));
        }

        /// <summary>
        /// Deletes the temporary working directory after each test.
        /// </summary>
        [TearDown]
        public void Cleanup()
        {
            Directory.Delete(TEMPDIR, true);
        }

        #region Utility
        /// <summary>
        /// Unzips the supplied ByteString-represented zip archive to path.
        /// </summary>
        /// <param name="data">The zip archive, represented as a bytestring</param>
        /// <param name="path">The path to unzip to</param>
        private static void Unzip(ByteString data, string path)
        {
            Byte[] compressed = data.ToArray();

            using MemoryStream ms = new MemoryStream(compressed);
            using ZipArchive archive = new ZipArchive(ms);
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string fullPath = Path.Combine(path, entry.FullName);
                string directory = Path.GetDirectoryName(fullPath);

                // If the parent directory of the current entry did not exist, we
                // create it. A second call to create the same directory will not
                // result in any action.
                if (directory != null)
                {
                    Directory.CreateDirectory(directory);
                }

                entry.ExtractToFile(fullPath, true);
            }
        }

        /// <summary>
        /// Roughly compares two directories, also see source.
        /// </summary>
        /// <see href="https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/how-to-compare-the-contents-of-two-folders-linq">Source</see>
        /// <returns>Whether the directories are the same</returns>
        private static bool CompareDirectories(string directoryA, string directoryB)
        {
            DirectoryInfo dirA = new DirectoryInfo(directoryA);
            DirectoryInfo dirB = new DirectoryInfo(directoryB);

            IEnumerable<FileInfo> listA = dirA.GetFiles("*.*", SearchOption.AllDirectories);
            IEnumerable<FileInfo> listB = dirB.GetFiles("*.*", SearchOption.AllDirectories);

            //A custom file comparer defined below  
            FileCompare myFileCompare = new FileCompare();

            // This query determines whether the two folders contain  
            // identical file lists, based on the custom file comparer  
            // that is defined in the FileCompare class.  
            // The query executes immediately because it returns a bool.  
            return listA.SequenceEqual(listB, myFileCompare);
        }
    }

    // This implementation defines a very simple comparison  
    // between two FileInfo objects. It only compares the name  
    // of the files being compared and their length in bytes.  
    class FileCompare : System.Collections.Generic.IEqualityComparer<System.IO.FileInfo>
    {
        public FileCompare() { }

        public bool Equals(System.IO.FileInfo f1, System.IO.FileInfo f2)
        {
            return (f1.Name == f2.Name &&
                    f1.Length == f2.Length);
        }

        // Return a hash that reflects the comparison criteria. According to the
        // rules for IEqualityComparer<T>, if Equals is true, then the hash codes must  
        // also be equal. Because equality as defined here is a simple value equality, not  
        // reference identity, it is possible that two or more objects will produce the same  
        // hash code.  
        public int GetHashCode(System.IO.FileInfo fi)
        {
            string s = $"{fi.Name}{fi.Length}";
            return s.GetHashCode();
        }
    }

#endregion
}
