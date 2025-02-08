using Fiap.FileCut.Core.Applications;
using Fiap.FileCut.Core.Exceptions;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects.Enums;
using Fiap.FileCut.Infra.Storage.Shared.Models;
using Moq;
using System.Text;

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
        var fileStream = new MemoryStream([1, 2, 3]);
        var formFile = new FileStreamResult(videoName, fileStream);
        fileService.Setup(x => x.GetFileAsync(guid, videoName, CancellationToken.None)).ReturnsAsync(formFile);

        // Act
        var result = await gestaoApplication.GetVideoAsync(guid, videoName, CancellationToken.None);

        // Assert
        Assert.Equal(formFile, result);
    }

    [Fact]
    public async Task GetVideoAsync_WhenFileNotFound_ReturnsEntityNotFoundException()
    {
        // Arrange
        var fileService = new Mock<IFileService>();
        var gestaoApplication = new GestaoApplication(fileService.Object);
        var guid = Guid.NewGuid();
        var videoName = "video.mp4";
        fileService.Setup(x => x.GetFileAsync(guid, videoName, CancellationToken.None)).ThrowsAsync(new FileNotFoundException());
        // Act
        async Task Act() => await gestaoApplication.GetVideoAsync(guid, videoName, CancellationToken.None);
        // Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(Act);
    }

    [Fact]
    public async Task GetVideoMetadataAsync_WhenCalled_ReturnsVideoMetadata()
    {
        // Arrange
        var fileService = new Mock<IFileService>();
        var gestaoApplication = new GestaoApplication(fileService.Object);
        var guid = Guid.NewGuid();
        var videoName = "video.mp4";
        var videoStream = new MemoryStream([1, 2, 3]);
        var videoFile = new FileStreamResult(videoName, videoStream);
        fileService.Setup(x => x.GetFileAsync(guid, videoName, CancellationToken.None)).ReturnsAsync(videoFile);
        fileService.Setup(x => x.GetFileAsync(guid, $"{videoName}.state", CancellationToken.None)).ThrowsAsync(new FileNotFoundException());

        // Act
        var result = await gestaoApplication.GetVideoMetadataAsync(guid, videoName, CancellationToken.None);

        // Assert
        Assert.Equal(videoName, result.Name);
        Assert.Equal(VideoState.PENDING, result.State);
    }

    [Fact]
    public async Task GetVideoMetadataAsync_WhenFileNotFound_ReturnsEntityNotFoundException()
    {
        // Arrange
        var fileService = new Mock<IFileService>();
        var gestaoApplication = new GestaoApplication(fileService.Object);
        var guid = Guid.NewGuid();
        var videoName = "video.mp4";
        fileService.Setup(x => x.GetFileAsync(guid, videoName, CancellationToken.None)).ThrowsAsync(new FileNotFoundException());
        // Act
        async Task Act() => await gestaoApplication.GetVideoMetadataAsync(guid, videoName, CancellationToken.None);
        // Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(Act);
    }

    [Fact]
    public async Task ListAllVideosAsync_WhenCalled_ReturnsListOfVideoMetadata()
    {
        // Arrange
        var fileService = new Mock<IFileService>();
        var gestaoApplication = new GestaoApplication(fileService.Object);
        var guid = Guid.NewGuid();
        var videoName = "video.mp4";
        var videoStream = new MemoryStream([1, 2, 3]);
        var videoFile = new FileStreamResult(videoName, videoStream);
        var stateSrt = "FAILED";
        var stateFile = new FileStreamResult($"{videoName}.state", new MemoryStream(Encoding.UTF8.GetBytes(stateSrt)));

        fileService.Setup(x => x.GetFileNamesAsync(guid, CancellationToken.None)).ReturnsAsync([videoName]);
        fileService.Setup(x => x.GetFileAsync(guid, videoName, CancellationToken.None)).ReturnsAsync(videoFile);
        fileService.Setup(x => x.GetFileAsync(guid, $"{videoName}.state", CancellationToken.None)).ReturnsAsync(stateFile);

        // Act
        var result = await gestaoApplication.ListAllVideosAsync(guid, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(videoName, result[0].Name);
        Assert.Equal(stateSrt, result[0].State.ToString());
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
        var result = await gestaoApplication.ListAllVideosAsync(guid, CancellationToken.None);
        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

}
