using Fiap.FileCut.Core.Interfaces.Repository;
using Fiap.FileCut.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fiap.FileCut.Core.UnitTests.Services;

public class FileServiceUnitTests
{
	[Fact]
	public async Task DeleteFileAsync_WhenFileDeletedSuccessfully_ShouldReturnTrue()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var fileName = "testfile.txt";
		var cancellationToken = CancellationToken.None;

		var fileRepositoryMock = new Mock<IFileRepository>();
		fileRepositoryMock
			.Setup(repo => repo.DeleteAsync(userId, fileName, cancellationToken))
			.ReturnsAsync(true);

		var loggerMock = new Mock<ILogger<FileService>>();
		var fileService = new FileService(fileRepositoryMock.Object, loggerMock.Object);

		// Act
		var result = await fileService.DeleteFileAsync(userId, fileName, cancellationToken);

		// Assert
		Assert.True(result);
		fileRepositoryMock.Verify(repo => repo.DeleteAsync(userId, fileName, cancellationToken), Times.Once); 
	}

	[Fact]
	public async Task DeleteFileAsync_WhenFileDeletionFails_ShouldReturnFalse()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var fileName = "testfile.txt";
		var cancellationToken = CancellationToken.None;

		var fileRepositoryMock = new Mock<IFileRepository>();
		fileRepositoryMock
			.Setup(repo => repo.DeleteAsync(userId, fileName, cancellationToken))
			.ReturnsAsync(false); 

		var loggerMock = new Mock<ILogger<FileService>>();
		var fileService = new FileService(fileRepositoryMock.Object, loggerMock.Object);

		// Act
		var result = await fileService.DeleteFileAsync(userId, fileName, cancellationToken);

		// Assert
		Assert.False(result);
		fileRepositoryMock.Verify(repo => repo.DeleteAsync(userId, fileName, cancellationToken), Times.Once);
	}

	[Fact]
	public async Task DeleteFileAsync_WhenExceptionOccurs_ShouldThrowInvalidOperationException()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var fileName = "testfile.txt";
		var cancellationToken = CancellationToken.None;

		var fileRepositoryMock = new Mock<IFileRepository>();
		fileRepositoryMock
			.Setup(repo => repo.DeleteAsync(userId, fileName, cancellationToken))
			.ThrowsAsync(new Exception("Simulação de erro"));

		var loggerMock = new Mock<ILogger<FileService>>();
		var fileService = new FileService(fileRepositoryMock.Object, loggerMock.Object);

		// Act & Assert
		var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
			await fileService.DeleteFileAsync(userId, fileName, cancellationToken));

		Assert.Equal($"[{nameof(FileService)}] - Error deleting file. User: {userId}, File: {fileName}", exception.Message);
		fileRepositoryMock.Verify(repo => repo.DeleteAsync(userId, fileName, cancellationToken), Times.Once);
	}

	[Fact]
	public async Task GetFileAsync_WhenFileExists_ShouldReturnFile()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var fileName = "testfile.txt";
		var cancellationToken = CancellationToken.None;

		var fileRepositoryMock = new Mock<IFileRepository>();
		var fileMock = new Mock<IFormFile>();
		fileRepositoryMock
			.Setup(repo => repo.GetAsync(userId, fileName, cancellationToken))
			.ReturnsAsync(fileMock.Object);

		var loggerMock = new Mock<ILogger<FileService>>();
		var fileService = new FileService(fileRepositoryMock.Object, loggerMock.Object);

		var result = await fileService.GetFileAsync(userId, fileName, cancellationToken);

		Assert.Equal(fileMock.Object, result);
		fileRepositoryMock.Verify(repo => repo.GetAsync(userId, fileName, cancellationToken), Times.Once); 
	}

	[Fact]
	public async Task GetFileAsync_WhenFileNotFound_ShouldThrowFileNotFoundException()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var fileName = "testfile.txt";
		var cancellationToken = CancellationToken.None;

		var fileRepositoryMock = new Mock<IFileRepository>();
		fileRepositoryMock
			.Setup(repo => repo.GetAsync(userId, fileName, cancellationToken))
			.ReturnsAsync((IFormFile)null);

		var loggerMock = new Mock<ILogger<FileService>>();
		var fileService = new FileService(fileRepositoryMock.Object, loggerMock.Object);

		// Act & Assert
		var exception = await Assert.ThrowsAsync<FileNotFoundException>(async () =>
			await fileService.GetFileAsync(userId, fileName, cancellationToken));

		Assert.Equal($"The file '{fileName}' for user '{userId}' was not found.", exception.Message);
		fileRepositoryMock.Verify(repo => repo.GetAsync(userId, fileName, cancellationToken), Times.Once); 
	}

	[Fact]
	public async Task GetFileAsync_WhenExceptionOccurs_ShouldThrowException()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var fileName = "testfile.txt";
		var cancellationToken = CancellationToken.None;

		var fileRepositoryMock = new Mock<IFileRepository>();
		fileRepositoryMock
			.Setup(repo => repo.GetAsync(userId, fileName, cancellationToken))
			.ThrowsAsync(new Exception("Simulação de erro"));

		var loggerMock = new Mock<ILogger<FileService>>();
		var fileService = new FileService(fileRepositoryMock.Object, loggerMock.Object);

		// Act & Assert
		var exception = await Assert.ThrowsAsync<Exception>(async () =>
			await fileService.GetFileAsync(userId, fileName, cancellationToken));

		fileRepositoryMock.Verify(repo => repo.GetAsync(userId, fileName, cancellationToken), Times.Once); 
	}

	[Fact]
	public async Task GetFileNamesAsync_WhenFilesExist_ShouldReturnFileNames()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var cancellationToken = CancellationToken.None;

		var fileRepositoryMock = new Mock<IFileRepository>();
		var fileList = new List<IFormFile>
		{
			new FormFile(new MemoryStream(), 0, 10, "file", "testfile1.txt"),
			new FormFile(new MemoryStream(), 0, 10, "file", "testfile2.txt")
		};

		fileRepositoryMock
			.Setup(repo => repo.GetAllAsync(userId, cancellationToken))
			.ReturnsAsync(fileList);

		var fileService = new FileService(fileRepositoryMock.Object, Mock.Of<ILogger<FileService>>());

		// Act & Assert
		var result = await fileService.GetFileNamesAsync(userId, cancellationToken);

		Assert.Equal(2, result.Count);
		Assert.Contains("testfile1.txt", result);
		Assert.Contains("testfile2.txt", result);
		fileRepositoryMock.Verify(repo => repo.GetAllAsync(userId, cancellationToken), Times.Once);
	}

	[Fact]
	public async Task GetFileNamesAsync_WhenNoFilesExist_ShouldReturnEmptyList()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var cancellationToken = CancellationToken.None;

		var fileRepositoryMock = new Mock<IFileRepository>();
		fileRepositoryMock
			.Setup(repo => repo.GetAllAsync(userId, cancellationToken))
			.ReturnsAsync(new List<IFormFile>());

		// Act & Assert
		var fileService = new FileService(fileRepositoryMock.Object, Mock.Of<ILogger<FileService>>());

		var result = await fileService.GetFileNamesAsync(userId, cancellationToken);

		Assert.Empty(result);
		fileRepositoryMock.Verify(repo => repo.GetAllAsync(userId, cancellationToken), Times.Once);
	}

	[Fact]
	public async Task GetFileNamesAsync_WhenExceptionOccurs_ShouldThrowInvalidOperationException()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var cancellationToken = CancellationToken.None;

		var fileRepositoryMock = new Mock<IFileRepository>();
		fileRepositoryMock
			.Setup(repo => repo.GetAllAsync(userId, cancellationToken))
			.ThrowsAsync(new Exception("Simulação de erro"));

		var fileService = new FileService(fileRepositoryMock.Object, Mock.Of<ILogger<FileService>>());

		// Act & Assert
		var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
			await fileService.GetFileNamesAsync(userId, cancellationToken));

		Assert.Equal($"[{nameof(FileService)}] - Error while listing file names. User: {userId}", exception.Message);
		fileRepositoryMock.Verify(repo => repo.GetAllAsync(userId, cancellationToken), Times.Once);
	}

}
