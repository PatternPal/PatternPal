#region

using System.IO.Compression;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using EventType = PatternPal.LoggingServer.EventType;

#endregion

namespace PatternPal.LoggingServerTests
{
    // TODO Comment
    public class LoggerServiceTests
    {
        private Mock<ILogger<LoggerService>> _loggerMock;
        private Mock<EventRepository> _repositoryMock;
        private LoggerService _service;

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

    [TearDown]
    public void TearDown()
    {
        string basePath = Path.Combine(Directory.GetCurrentDirectory(), "CodeStates");
        if (Directory.Exists(basePath))
        {
            Directory.Delete(basePath, true);
        }
    }

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

        [Test]
        public void Log_InvalidDateTime_ThrowsInvalidArgument()
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
            Assert.ThrowsAsync<RpcException>(() => _service.Log(request, Mock.Of<ServerCallContext>()));
        }

        [Test]
        public void Log_InvalidSessionId_ThrowsInvalidArgument()
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
            Assert.ThrowsAsync<RpcException>(() => _service.Log(request, Mock.Of<ServerCallContext>()));
        }

        [Test]
        public void Log_UnknownEventType_ThrowsInvalidArgument()
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
            Assert.ThrowsAsync<RpcException>(() => _service.Log(request, Mock.Of<ServerCallContext>()));
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
