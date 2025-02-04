using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.IO.Compression;
using System.Threading.Tasks;
using FFMpegCore;
using Fiap.FileCut.Core.Handlers;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Fiap.FileCut.Tests.Handlers
{
    public class VideoProcessingHandlerTests
    {
        private readonly Mock<INotifyService> _mockNotifyService;
        private readonly Mock<ILogger<VideoProcessingHandler>> _mockLogger;
        private readonly IOptions<ProcessingOptions> _mockOptions;
        private readonly MockFileSystem _mockFileSystem;

        public VideoProcessingHandlerTests()
        {
            _mockNotifyService = new Mock<INotifyService>();
            _mockLogger = new Mock<ILogger<VideoProcessingHandler>>();

            _mockOptions = Options.Create(new ProcessingOptions
            {
                WorkingDirectory = "C:\\Temp",
                FrameIntervalSeconds = 5,
                FrameWidth = 1920,
                FrameHeight = 1080
            });

            _mockFileSystem = new MockFileSystem();
        }

        [Fact]
        public async Task HandleAsync_ShouldUpdateStatus_ProcessingAndCompleted()
        {
            // Arrange
            var handler = new VideoProcessingHandler(_mockNotifyService.Object, _mockLogger.Object, _mockOptions);
            var userId = Guid.NewGuid();
            var videoPath = "C:\\Videos\\video.mp4";

            var context = new NotifyContext<string>(videoPath, userId);

            // Act
            await handler.HandleAsync(context);

            // Assert
            _mockNotifyService.Verify(x => x.NotifyAsync(It.Is<NotifyContext<ProcessingStatus>>(s =>
                s.Context.Status == "PROCESSING" &&
                s.UserId == userId)), Times.AtLeastOnce);

            _mockNotifyService.Verify(x => x.NotifyAsync(It.Is<NotifyContext<ProcessingStatus>>(s =>
                s.Context.Status == "COMPLETED" &&
                s.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ShouldCreateOutputDirectory()
        {
            // Arrange
            var handler = new VideoProcessingHandler(_mockNotifyService.Object, _mockLogger.Object, _mockOptions);
            var userId = Guid.NewGuid();
            var videoPath = "C:\\Videos\\video.mp4";

            var context = new NotifyContext<string>(videoPath, userId);

            // Act
            await handler.HandleAsync(context);

            // Assert
            var outputFolder = Path.Combine(_mockOptions.Value.WorkingDirectory, userId.ToString(), "frames");
            Assert.True(Directory.Exists(outputFolder));
        }

        [Fact]
        public async Task HandleAsync_ShouldGenerateZipFile()
        {
            // Arrange
            var handler = new VideoProcessingHandler(_mockNotifyService.Object, _mockLogger.Object, _mockOptions);
            var userId = Guid.NewGuid();
            var videoPath = "C:\\Videos\\video.mp4";

            var context = new NotifyContext<string>(videoPath, userId);

            // Act
            await handler.HandleAsync(context);

            // Assert
            var zipFilePath = Path.Combine(_mockOptions.Value.WorkingDirectory, userId.ToString(), "frames.zip");
            Assert.True(File.Exists(zipFilePath));
        }

        [Fact]
        public async Task HandleAsync_ShouldLogErrorAndUpdateStatusOnException()
        {
            // Arrange
            _mockNotifyService
                .Setup(x => x.NotifyAsync(It.IsAny<NotifyContext<ProcessingStatus>>()))
                .ThrowsAsync(new Exception("Erro de teste"));

            var handler = new VideoProcessingHandler(_mockNotifyService.Object, _mockLogger.Object, _mockOptions);
            var userId = Guid.NewGuid();
            var videoPath = "C:\\Videos\\video.mp4";
            var context = new NotifyContext<string>(videoPath, userId);

            // Act
            await handler.HandleAsync(context);

            // Assert
            _mockLogger.Verify(x => x.LogError(It.IsAny<Exception>(), "Erro durante o processamento do vídeo para o usuário {UserId}", userId), Times.Once);
            _mockNotifyService.Verify(x => x.NotifyAsync(It.Is<NotifyContext<ProcessingStatus>>(s =>
                s.Context.Status == "FAILED" &&
                s.UserId == userId)), Times.Once);
        }
    }
}
