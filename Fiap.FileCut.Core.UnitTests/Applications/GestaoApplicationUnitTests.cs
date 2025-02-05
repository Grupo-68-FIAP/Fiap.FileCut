using Fiap.FileCut.Core.Applications;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Infra.Storage.Shared.Models;
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
		var fileStream = new MemoryStream(new byte[] { 1, 2, 3 }); 
		var formFile = new FileStreamResult(videoName, fileStream); 
		fileService.Setup(x => x.GetFileAsync(guid, videoName, CancellationToken.None)).ReturnsAsync(formFile);

		// Act
		var result = await gestaoApplication.GetVideoAsync(guid, videoName, CancellationToken.None);

		// Assert
		Assert.Equal(formFile, result);
	}

	[Fact]
	public async Task GetVideoMetadataAsync_WhenCalled_ReturnsVideoMetadata()
	{
		// Arrange
		var fileService = new Mock<IFileService>();
		var gestaoApplication = new GestaoApplication(fileService.Object);
		var guid = Guid.NewGuid();
		var videoName = "video.mp4";
		var fileStream = new MemoryStream(new byte[] { 1, 2, 3 }); 
		var formFile = new FileStreamResult(videoName, fileStream); 
		fileService.Setup(x => x.GetFileAsync(guid, videoName, CancellationToken.None)).ReturnsAsync(formFile);

		// Act
		var result = await gestaoApplication.GetVideoMetadataAsync(guid, videoName, CancellationToken.None);

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
		var fileStream = new MemoryStream(new byte[] { 1, 2, 3 }); 
		var formFile = new FileStreamResult(videoName, fileStream); 

		fileService.Setup(x => x.GetFileNamesAsync(guid, CancellationToken.None)).ReturnsAsync(new List<string> { videoName });
		fileService.Setup(x => x.GetFileAsync(guid, videoName, CancellationToken.None)).ReturnsAsync(formFile);

		// Act
		var result = await gestaoApplication.ListAllVideosAsync(guid, CancellationToken.None);

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
        var result = await gestaoApplication.ListAllVideosAsync(guid, CancellationToken.None);
        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

}
