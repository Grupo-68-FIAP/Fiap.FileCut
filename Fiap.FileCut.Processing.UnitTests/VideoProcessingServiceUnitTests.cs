using Fiap.FileCut.Processing.Services;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Fiap.FileCut.Processing.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fiap.FileCut.Processing.UnitTests
{
    public class VideoProcessingServiceUnitTests
    {
        private readonly Mock<ILogger<VideoProcessingService>> _mockLogger;
        private readonly Mock<IOptions<ProcessingOptions>> _mockOptions;
        private readonly ProcessingOptions _processingOptions;

        public VideoProcessingServiceUnitTests()
        {
            _mockLogger = new Mock<ILogger<VideoProcessingService>>();
            _mockOptions = new Mock<IOptions<ProcessingOptions>>();
            _processingOptions = new ProcessingOptions
            {
                WorkingDirectory = "test_working_directory",
                FrameIntervalSeconds = 1,
                FrameWidth = 640,
                FrameHeight = 480
            };
            _mockOptions.Setup(o => o.Value).Returns(_processingOptions);
        }

        [Fact]
        public async Task ProcessVideoAsync_Should_Process_Video_And_Return_ZipFilePath()
        {
            // Arrange
            var service = new VideoProcessingService(_mockLogger.Object, _mockOptions.Object);
            var userId = Guid.NewGuid();
            var videoPath = "test_video.mp4";

            // Act
            var result = await service.ProcessVideoAsync(userId, videoPath);

            // Assert
            Assert.NotNull(result);
            Assert.EndsWith(".zip", result);
        }

        [Fact]
        public async Task ProcessVideoAsync_Should_Throw_VideoProcessingException_On_Error()
        {
            // Arrange
            var service = new VideoProcessingService(_mockLogger.Object, _mockOptions.Object);
            var userId = Guid.NewGuid();
            var invalidVideoPath = "invalid_video_path.mp4";

            // Act & Assert
            await Assert.ThrowsAsync<VideoProcessingException>(() => service.ProcessVideoAsync(userId, invalidVideoPath));
        }

        [Fact]
        public async Task ProcessVideoAsync_Should_Log_Error_On_Exception()
        {
            // Arrange
            var service = new VideoProcessingService(_mockLogger.Object, _mockOptions.Object);
            var userId = Guid.NewGuid();
            var invalidVideoPath = "invalid_video_path.mp4";

            // Act
            await Assert.ThrowsAsync<VideoProcessingException>(() => service.ProcessVideoAsync(userId, invalidVideoPath));

            // Assert
            _mockLogger.Verify(
                l => l.LogError(It.IsAny<Exception>(), "Erro durante o processamento do vídeo para o usuário {UserId}", userId),
                Times.Once);
        }

        [Fact]
        public async Task ProcessVideoAsync_Should_Throw_OperationCanceledException_When_Cancelled()
        {
            // Arrange
            var service = new VideoProcessingService(_mockLogger.Object, _mockOptions.Object);
            var userId = Guid.NewGuid();
            var videoPath = "test_video.mp4";
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => service.ProcessVideoAsync(userId, videoPath, cts.Token));
        }

        [Fact]
        public async Task ProcessVideoAsync_Should_Throw_FileNotFoundException_When_Video_Not_Found()
        {
            // Arrange
            var service = new VideoProcessingService(_mockLogger.Object, _mockOptions.Object);
            var userId = Guid.NewGuid();
            var nonExistentVideoPath = "non_existent_video.mp4";

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => service.ProcessVideoAsync(userId, nonExistentVideoPath));
        }

        [Fact]
        public async Task ProcessVideoAsync_Should_Throw_VideoProcessingException_For_Invalid_Format()
        {
            // Arrange
            var service = new VideoProcessingService(_mockLogger.Object, _mockOptions.Object);
            var userId = Guid.NewGuid();
            var invalidFormatVideoPath = "invalid_format_video.xyz";

            // Act & Assert
            await Assert.ThrowsAsync<VideoProcessingException>(() => service.ProcessVideoAsync(userId, invalidFormatVideoPath));
        }
    }
}