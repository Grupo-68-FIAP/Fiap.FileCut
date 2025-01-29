using Fiap.FileCut.Core.Applications;
using Fiap.FileCut.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Fiap.FileCut.Core.UnitTests.Applications;

public class GestaoApplicationUnitTests
{
    [Fact]
    public async Task GetVideoAsync_WhenCalled_ReturnsVideo()
    {
        // Arrange
        var fileService = new Mock<IFileService>();
        var gestaoApplication = new GestaoApplication(fileService.Object);
        var guid = Guid.NewGuid();
        var videoName = "video.mp4";
        var formFile = new Mock<IFormFile>();
        fileService.Setup(x => x.GetFileAsync(guid, videoName, CancellationToken.None)).ReturnsAsync(formFile.Object);
        // Act
        var result = await gestaoApplication.GetVideoAsync(guid, videoName);
        // Assert
        Assert.Equal(formFile.Object, result);
    }

    [Fact]
    public async Task GetVideoMetadataAsync_WhenCalled_ReturnsVideoMetadata()
    {
        // Arrange
        var fileService = new Mock<IFileService>();
        var gestaoApplication = new GestaoApplication(fileService.Object);
        var guid = Guid.NewGuid();
        var videoName = "video.mp4";
        var formFile = new Mock<IFormFile>();
        formFile.Setup(x => x.FileName).Returns(videoName);
        fileService.Setup(x => x.GetFileAsync(guid, videoName, CancellationToken.None)).ReturnsAsync(formFile.Object);
        // Act
        var result = await gestaoApplication.GetVideoMetadataAsync(guid, videoName);
        // Assert
        Assert.NotNull(result);
        Assert.Equal(videoName, result.Name);
    }

    [Fact]
    public async Task ListAllVideosAsync_WhenCalled_ReturnsListOfVideoMetadata()
    {
        // Arrange
        var fileService = new Mock<IFileService>();
        var gestaoApplication = new GestaoApplication(fileService.Object);
        var guid = Guid.NewGuid();
        var videoName = "video.mp4";
        var formFile = new Mock<IFormFile>();
        formFile.Setup(x => x.FileName).Returns(videoName);
        fileService.Setup(x => x.GetFileNamesAsync(guid, CancellationToken.None)).ReturnsAsync([videoName]);
        fileService.Setup(x => x.GetFileAsync(guid, videoName, CancellationToken.None)).ReturnsAsync(formFile.Object);
        // Act
        var result = await gestaoApplication.ListAllVideosAsync(guid);
        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(videoName, result[0].Name);
    }

    [Fact]
    public async Task ListAllVideosAsync_WhenNoFiles_ReturnsEmptyList()
    {
        // Arrange
        var fileService = new Mock<IFileService>();
        var gestaoApplication = new GestaoApplication(fileService.Object);
        var guid = Guid.NewGuid();
        fileService.Setup(x => x.GetFileNamesAsync(guid, CancellationToken.None)).ReturnsAsync([]);
        // Act
        var result = await gestaoApplication.ListAllVideosAsync(guid);
        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

}
