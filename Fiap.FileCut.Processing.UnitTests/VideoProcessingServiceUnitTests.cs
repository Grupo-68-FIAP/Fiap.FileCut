using Fiap.FileCut.Processing.Services;
using FFMpegCore;
using Moq;
using System.IO.Compression;
using Xunit;
using Fiap.FileCut.Processing.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Fiap.FileCut.Core.Objects;

namespace Fiap.FileCut.Processing.UnitTests;

public class VideoProcessingServiceTests
{
    private readonly Mock<IOptions<ProcessingOptions>> _mockOptions;
    private readonly ProcessingOptions _options;
    private readonly VideoProcessingService _service;

    public VideoProcessingServiceTests()
    {
        _mockOptions = new Mock<IOptions<ProcessingOptions>>();
        _options = new ProcessingOptions
        {
            WorkingDirectory = "test_processing",
            FrameIntervalSeconds = 1,
            FrameWidth = 1920,
            FrameHeight = 1080
        };

        _mockOptions.Setup(o => o.Value).Returns(_options);
        _service = new VideoProcessingService(Mock.Of<ILogger<VideoProcessingService>>(), _mockOptions.Object);
    }

    [Fact]
    public async Task ProcessVideoAsync_ShouldCreateOutputStructure()
    {
        var videoPath = "test.mp4";
        var userId = Guid.NewGuid();

        var mockVideoInfo = new Mock<IMediaAnalysis>();
        mockVideoInfo.SetupGet(v => v.Duration).Returns(TimeSpan.FromSeconds(5));

        await _service.ProcessVideoAsync(videoPath, userId);
        var expectedZipPath = Path.Combine(_options.WorkingDirectory, userId.ToString(), "frames.zip");
        Assert.True(File.Exists(expectedZipPath));

        Directory.Delete(Path.Combine(_options.WorkingDirectory, userId.ToString()), true);
    }

    [Fact]
    public async Task ProcessVideoAsync_ShouldThrowOnInvalidVideoPath()
    {
        var invalidPath = "invalid.mp4";
        var userId = Guid.NewGuid();

        await Assert.ThrowsAsync<VideoProcessingException>(
            () => _service.ProcessVideoAsync(invalidPath, userId));
    }

    [Fact]
    public async Task ProcessVideoAsync_ShouldGenerateCorrectNumberOfFrames()
    {
        var videoPath = "test.mp4";
        var userId = Guid.NewGuid();
        var duration = TimeSpan.FromSeconds(5);

        var mockVideoInfo = new Mock<IMediaAnalysis>();
        mockVideoInfo.SetupGet(v => v.Duration).Returns(duration);

        await _service.ProcessVideoAsync(videoPath, userId);

        var framesDir = Path.Combine(_options.WorkingDirectory, userId.ToString(), "frames");
        var frameCount = Directory.GetFiles(framesDir, "*.jpg").Length;
        Assert.Equal((int) (duration.TotalSeconds / _options.FrameIntervalSeconds), frameCount);
    }

    [Fact]
    public async Task ProcessVideoAsync_ShouldHandleFFmpegErrors()
    {
        var videoPath = "test-fiap.mp4";
        var userId = Guid.NewGuid();

        var mockVideoInfo = new Mock<IMediaAnalysis>();
        mockVideoInfo.SetupGet(v => v.Duration).Throws(new Exception("FFmpeg error"));

        await Assert.ThrowsAsync<VideoProcessingException>(
            () => _service.ProcessVideoAsync(videoPath, userId));
    }

    [Fact]
    public async Task ProcessVideoAsync_ShouldCreateValidZipFile()
    {
        var videoPath = "test-processing.mp4";
        var userId = Guid.NewGuid();

        await _service.ProcessVideoAsync(videoPath, userId);

        var zipPath = Path.Combine(_options.WorkingDirectory, userId.ToString(), "frames.zip");
        using var archive = ZipFile.OpenRead(zipPath);
        Assert.NotEmpty(archive.Entries);
    }
}