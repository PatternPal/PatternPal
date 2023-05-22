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
        /// TODO
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
        /// TODO
        /// </summary>
        /// <param name="path"></param>
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
        /// TODO
        /// </summary>
        /// <param name="path"></param>
        /// <param name="expected"></param>
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
        /// TODO
        /// </summary>
        [TearDown]
        public void Cleanup()
        {
            Directory.Delete(TEMPDIR, true);
        }

        #region Utility
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        private static void Unzip(ByteString data, string path)
        {
            Byte[] compressed = data.ToArray();

            using MemoryStream ms = new MemoryStream(compressed);
            using ZipArchive archive = new ZipArchive(ms);
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string fullPath = Path.Combine(path, entry.FullName);
                string directory = Path.GetDirectoryName(fullPath);

                if (directory != null)
                {
                    Directory.CreateDirectory(directory);
                }

                entry.ExtractToFile(fullPath, true);
            }
        }

        /// <summary>
        /// TODO
        /// Include src
        /// </summary>
        /// <param name="directoryA"></param>
        /// <param name="directoryB"></param>
        /// <returns></returns>
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
