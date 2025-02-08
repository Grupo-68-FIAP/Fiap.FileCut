using Fiap.FileCut.Core.Objects;
using Fiap.FileCut.Processing.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.IO.Compression;

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
                FrameIntervalSeconds = 30,
                FrameWidth = 640,
                FrameHeight = 480
            };
            _mockOptions.Setup(o => o.Value).Returns(_processingOptions);
        }

        [Fact]
        public async Task ProcessVideoAsync_WithValidParameters_ShouldReturnZipFilePath()
        {
            // Arrange
            var videoPath = "sample.mp4";
            using var videoStream = File.OpenRead(Path.Combine(Directory.GetCurrentDirectory(), "Resources", videoPath));
            var videoProcessingService = new VideoProcessingService(_mockLogger.Object, _mockOptions.Object);
            // Act
            var zipStream = await videoProcessingService.ProcessVideoAsync(videoStream, CancellationToken.None);
            // Assert
            Assert.NotNull(zipStream);

            using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read, leaveOpen: true);
            // O video de exemplo tem 02:39 de duração, com intervalo de 30 segundos, teremos 6 frames
            Assert.Equal(6, archive.Entries.Count);
        }
    }
}