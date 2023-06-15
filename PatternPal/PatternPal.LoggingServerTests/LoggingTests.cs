#region

using System.IO.Compression;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using EventType = PatternPal.LoggingServer.EventType;

#endregion

namespace PatternPal.LoggingServerTests
{
    /// <summary>
    /// LoggingService tests is a class that tests the LoggingService class. It uses a mock repository to test the database and a mock logger to test the logging.
    /// </summary>
    public class LoggerServiceTests
    {
        
        private Mock<ILogger<LoggerService>> _loggerMock;
        private Mock<EventRepository> _repositoryMock;
        private LoggerService _service;

        /// <summary>
        /// Setup is a method that is called before each test. It creates a mock logger and a mock repository with a inmemory database and then creates a new LoggingService object.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<LoggerService>>();

            DbContextOptions<ProgSnap2ContextClass> options = new DbContextOptionsBuilder<ProgSnap2ContextClass>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;
            ProgSnap2ContextClass context = new(options);
            _repositoryMock = new Mock<EventRepository>(context);
            _service = new LoggerService(_loggerMock.Object, _repositoryMock.Object);
        }
    /// <summary>
    /// TearDown is a method that is called after each test. It deletes the CodeStates directory that is created by the LoggingService.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        string basePath = Path.Combine(Directory.GetCurrentDirectory(), "CodeStates");
        if (Directory.Exists(basePath))
        {
            Directory.Delete(basePath, true);
        }
    }
        /// <summary>
        /// This test checks if the Log method calls the repository's Insert method when given a valid request. This request is created with only the required fields.
        /// </summary>
        [Test]
        public async Task Log_ValidRequest_InsertsEventIntoDatabase()
        {
            // Arrange
            LogRequest request = new LogRequest
            {
                EventId = Guid.NewGuid().ToString(),
                SessionId = Guid.NewGuid().ToString(),
                SubjectId = Guid.NewGuid().ToString(),
                EventType = EventType.EvtSessionStart,
                ClientTimestamp = DateTimeOffset.Now.ToString("o"),
                ProjectId = "TestProject",
            };

            // Act
            await _service.Log(request, Mock.Of<ServerCallContext>());

            // Assert
            _repositoryMock.Verify(r => r.Insert(It.IsAny<ProgSnap2Event>()), Times.Once);
        }

        /// <summary>
        /// This test checks if the log method returns the correct statuscode when given an invalid request. In this case the datetime is invalid.
        /// </summary>
        [Test]
        public void Log_InvalidDateTime_InvalidArgument()
        {
            // Arrange
            LogRequest request = new LogRequest
            {
                EventId = Guid.NewGuid().ToString(),
                SessionId = Guid.NewGuid().ToString(),
                SubjectId = Guid.NewGuid().ToString(),
                EventType = EventType.EvtProjectOpen,
                ClientTimestamp = "invalid datetime",
                ProjectId = "TestProject",
                Data = ByteString.CopyFrom(1, 2, 3)
            };

            // Act & Assert
            // assert that response status code is 4
            Assert.That(_service.Log(request, Mock.Of<ServerCallContext>()).Result.Status, Is.EqualTo(LogStatusCodes.LscInvalidArguments));

        }

        /// <summary>
        /// This session checks if the log method returns the correct statuscode when given an invalid request. In this case the sessionid is invalid.
        /// </summary>
        [Test]
        public void Log_InvalidSessionId_InvalidArgument()
        {
            // Arrange
            LogRequest request = new LogRequest
            {
                EventId = Guid.NewGuid().ToString(),
                SessionId = "invalid session ID",
                SubjectId = Guid.NewGuid().ToString(),
                EventType = EventType.EvtProjectOpen,
                ClientTimestamp = DateTimeOffset.Now.ToString("o"),
                ProjectId = "TestProject",
                Data = ByteString.CopyFrom(new byte[] { 1, 2, 3 })
            };

            // Act & Assert
            Assert.That(_service.Log(request, Mock.Of<ServerCallContext>()).Result.Status, Is.EqualTo(LogStatusCodes.LscInvalidArguments));
        }

        /// <summary>
        /// This test checks if the log method returns the correct statuscode when given an invalid request. In this case the eventtype cannot be unknown.
        /// </summary>
        [Test]
        public void Log_UnknownEventType_InvalidArgument()
        {
            // Arrange
            LogRequest request = new LogRequest
            {
                EventId = Guid.NewGuid().ToString(),
                SessionId = Guid.NewGuid().ToString(),
                SubjectId = Guid.NewGuid().ToString(),
                EventType = EventType.EvtUnknown,
                ClientTimestamp = DateTimeOffset.Now.ToString("o"),
                ProjectId = "TestProject",
                Data = ByteString.CopyFrom(new byte[] { 1, 2, 3 })
            };

            // Act & Assert
            Assert.That(_service.Log(request, Mock.Of<ServerCallContext>()).Result.Status, Is.EqualTo(LogStatusCodes.LscInvalidArguments));
        }

        [Test]
        public async Task Log_ValidRequestWithData_SavesCodeStateToDisk()
        {
            // Arrange
            string filenameCs = Guid.NewGuid().ToString() + ".cs";
            LogRequest request = new()
            {
                EventId = Guid.NewGuid().ToString(),
                SessionId = Guid.NewGuid().ToString(),
                SubjectId = Guid.NewGuid().ToString(),
                EventType = EventType.EvtProjectClose,
                ClientTimestamp = DateTimeOffset.Now.ToString("o"),
                ProjectId = "TestProject",
                Data = ByteString.CopyFrom(CreateZipArchive(filenameCs))
            };

            // Act
            await _service.Log(request, Mock.Of<Grpc.Core.ServerCallContext>());

            // Assert
            string basePath = Path.Combine(Directory.GetCurrentDirectory(), "CodeStates");
            // get the most recent directory
            string codeStatePath = Directory.GetDirectories(basePath).OrderByDescending(d => d).First();

            // sleep for a bit to make sure the file is written to disk
            Assert.That(File.Exists(Path.Combine(codeStatePath, filenameCs)), Is.True);
        }

        /// <summary>
        /// Helper function to create a zip archive with a single file in it.
        /// The file is called filename and contains the text "test".
        /// </summary>
        /// <param name="filename">Filename of the file to create in the zip archive</param>
        /// <returns cref="byte[]">Byte array containing the zip archive</returns>
        private byte[] CreateZipArchive(string filename)
        {
            using MemoryStream ms = new();
            using ZipArchive archive = new(ms, ZipArchiveMode.Create, true);
            ZipArchiveEntry entry = archive.CreateEntry(filename);
            using StreamWriter writer = new(entry.Open());
            writer.Write("test");
            writer.Flush();
            writer.Close();
            archive.Dispose();
            return ms.ToArray();
        }
    }
}
