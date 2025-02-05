using Fiap.FileCut.Core.Interfaces.Repository;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Services;
using Fiap.FileCut.Infra.Storage.Shared.Models;
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

		var notifyService = new Mock<INotifyService>();
		var fileRepositoryMock = new Mock<IFileRepository>();
		fileRepositoryMock
			.Setup(repo => repo.DeleteAsync(userId, fileName, cancellationToken))
			.ReturnsAsync(true);

		var loggerMock = new Mock<ILogger<FileService>>();
		var fileService = new FileService(fileRepositoryMock.Object, notifyService.Object, loggerMock.Object);

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

		var notifyService = new Mock<INotifyService>();

		var fileRepositoryMock = new Mock<IFileRepository>();
		fileRepositoryMock
			.Setup(repo => repo.DeleteAsync(userId, fileName, cancellationToken))
			.ReturnsAsync(false); 

		var loggerMock = new Mock<ILogger<FileService>>();
		var fileService = new FileService(fileRepositoryMock.Object, notifyService.Object, loggerMock.Object);

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
		var notifyService = new Mock<INotifyService>();

		var fileRepositoryMock = new Mock<IFileRepository>();
		fileRepositoryMock
			.Setup(repo => repo.DeleteAsync(userId, fileName, cancellationToken))
			.ThrowsAsync(new Exception("Simulação de erro"));

		var loggerMock = new Mock<ILogger<FileService>>();
		var fileService = new FileService(fileRepositoryMock.Object, notifyService.Object, loggerMock.Object);

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

		var notifyService = new Mock<INotifyService>();

		var fileRepositoryMock = new Mock<IFileRepository>();
		var fileMock = new Mock<FileStreamResult>();
		fileRepositoryMock
			.Setup(repo => repo.GetAsync(userId, fileName, cancellationToken))
			.ReturnsAsync(fileMock.Object);

		var loggerMock = new Mock<ILogger<FileService>>();
		var fileService = new FileService(fileRepositoryMock.Object, notifyService.Object, loggerMock.Object);

		var result = await fileService.GetFileAsync(userId, fileName, cancellationToken);

		Assert.Equal(fileMock.Object, result);
		fileRepositoryMock.Verify(repo => repo.GetAsync(userId, fileName, cancellationToken), Times.Once); 
	}

	[Fact]
	public async Task GetFileAsync_WhenFileNotFound_ShouldThrowInvalidOperationException()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var fileName = "testfile.txt";
		var cancellationToken = CancellationToken.None;

		var fileRepositoryMock = new Mock<IFileRepository>();
		var notifyService = new Mock<INotifyService>();
		var loggerMock = new Mock<ILogger<FileService>>();
		var fileService = new FileService(fileRepositoryMock.Object, notifyService.Object, loggerMock.Object);

		// Simular o erro ao buscar o arquivo
		fileRepositoryMock.Setup(repo => repo.GetAsync(userId, fileName, cancellationToken))
						  .ThrowsAsync(new InvalidOperationException("[FileService] - Error while downloading file. User: " + userId));

		// Act & Assert
		var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
			await fileService.GetFileAsync(userId, fileName, cancellationToken));

		// Ajuste: Verificar se a mensagem contém a parte esperada
		Assert.Contains("[FileService] - Error while downloading file.", exception.Message);
		fileRepositoryMock.Verify(repo => repo.GetAsync(userId, fileName, cancellationToken), Times.Once);
	}

	[Fact]
	public async Task GetFileAsync_WhenExceptionOccurs_ShouldThrowInvalidOperationException()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var fileName = "testfile.txt";
		var cancellationToken = CancellationToken.None;

		var notifyService = new Mock<INotifyService>();

		var fileRepositoryMock = new Mock<IFileRepository>();
		fileRepositoryMock
			.Setup(repo => repo.GetAsync(userId, fileName, cancellationToken))
			.ThrowsAsync(new Exception("Simulação de erro"));

		var loggerMock = new Mock<ILogger<FileService>>();
		var fileService = new FileService(fileRepositoryMock.Object, notifyService.Object, loggerMock.Object);

		// Act & Assert
		var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
			await fileService.GetFileAsync(userId, fileName, cancellationToken));

		Assert.Equal("[FileService] - Error while downloading file. User: " + userId + ", File: " + fileName, exception.Message);
		fileRepositoryMock.Verify(repo => repo.GetAsync(userId, fileName, cancellationToken), Times.Once);
	}

	[Fact]
	public async Task GetFileNamesAsync_WhenFilesExist_ShouldReturnFileNames()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var cancellationToken = CancellationToken.None;

		var notifyService = new Mock<INotifyService>();

		// Mockando o repositório sem criar realmente arquivos
		var fileRepositoryMock = new Mock<IFileRepository>();

		// Simulando uma lista de resultados de arquivos
		var fileList = new List<FileStreamResult>
		{
			new FileStreamResult("testfile1.txt", new MemoryStream()),
			new FileStreamResult("testfile2.txt", new MemoryStream()) 
		};

		// Configuração do repositório mockado para retornar os arquivos simulados
		fileRepositoryMock
			.Setup(repo => repo.GetAllAsync(userId, cancellationToken))
			.ReturnsAsync(fileList);

		var fileService = new FileService(fileRepositoryMock.Object, notifyService.Object, Mock.Of<ILogger<FileService>>());

		// Act & Assert
		var result = await fileService.GetFileNamesAsync(userId, cancellationToken);

		// Verificação dos resultados esperados
		Assert.Equal(2, result.Count);
		Assert.Contains("testfile1.txt", result);
		Assert.Contains("testfile2.txt", result);

		// Verificando que o repositório foi chamado corretamente
		fileRepositoryMock.Verify(repo => repo.GetAllAsync(userId, cancellationToken), Times.Once);
	}


	[Fact]
	public async Task GetFileNamesAsync_WhenNoFilesExist_ShouldReturnEmptyList()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var cancellationToken = CancellationToken.None;
		var notifyService = new Mock<INotifyService>();

		var fileRepositoryMock = new Mock<IFileRepository>();
		fileRepositoryMock
			.Setup(repo => repo.GetAllAsync(userId, cancellationToken))
			.ReturnsAsync(new List<FileStreamResult>());

		// Act & Assert
		var fileService = new FileService(fileRepositoryMock.Object, notifyService.Object, Mock.Of<ILogger<FileService>>());

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
		var notifyService = new Mock<INotifyService>();

		var fileRepositoryMock = new Mock<IFileRepository>();
		fileRepositoryMock
			.Setup(repo => repo.GetAllAsync(userId, cancellationToken))
			.ThrowsAsync(new Exception("Simulação de erro"));

		var fileService = new FileService(fileRepositoryMock.Object, notifyService.Object, Mock.Of<ILogger<FileService>>());

		// Act & Assert
		var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
			await fileService.GetFileNamesAsync(userId, cancellationToken));

		Assert.Equal($"[{nameof(FileService)}] - Error while listing file names. User: {userId}", exception.Message);
		fileRepositoryMock.Verify(repo => repo.GetAllAsync(userId, cancellationToken), Times.Once);
	}

	[Fact]
	public async Task SaveFileAsync_WhenFileIsValid_ShouldSaveFileAndReturnTrue()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var fileName = "testfile.txt";
		var fileStream = new MemoryStream(new byte[10]);

		var notifyService = new Mock<INotifyService>();

		var cancellationToken = CancellationToken.None;
		var fileRepositoryMock = new Mock<IFileRepository>();
		fileRepositoryMock
			.Setup(repo => repo.UpdateAsync(userId, fileStream, fileName, cancellationToken))
			.ReturnsAsync(true);

		var fileService = new FileService(fileRepositoryMock.Object, notifyService.Object, Mock.Of<ILogger<FileService>>());

		// Act
		var result = await fileService.SaveFileAsync(userId, fileName, fileStream, cancellationToken);

		// Assert
		Assert.True(result);
		fileRepositoryMock.Verify(repo => repo.UpdateAsync(userId,fileStream, fileName, cancellationToken), Times.Once);
	}

	[Fact]
	public async Task SaveFileAsync_WhenSaveFails_ShouldReturnFalse()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var fileName = "testfile.txt";
		var fileStream = new MemoryStream(new byte[10]);

		var notifyService = new Mock<INotifyService>();

		var cancellationToken = CancellationToken.None;
		var fileRepositoryMock = new Mock<IFileRepository>();
		fileRepositoryMock
			.Setup(repo => repo.UpdateAsync(userId, fileStream, fileName, cancellationToken))
			.ReturnsAsync(false);

		var fileService = new FileService(fileRepositoryMock.Object, notifyService.Object, Mock.Of<ILogger<FileService>>());

		// Act
		var result = await fileService.SaveFileAsync(userId, fileName, fileStream, cancellationToken);

		// Assert
		Assert.False(result);
		fileRepositoryMock.Verify(repo => repo.UpdateAsync(userId, fileStream, fileName, cancellationToken), Times.Once);
	}

	[Fact]
	public async Task SaveFileAsync_WhenUnexpectedErrorOccurs_ShouldThrowInvalidOperationException()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var fileName = "testfile.txt";
		var fileStream = new MemoryStream(new byte[10]);

		var notifyService = new Mock<INotifyService>();

		var cancellationToken = CancellationToken.None;
		var fileRepositoryMock = new Mock<IFileRepository>();
		fileRepositoryMock
			.Setup(repo => repo.UpdateAsync(userId, fileStream, fileName, cancellationToken))
			.ThrowsAsync(new Exception("Simulação de erro"));

		var fileService = new FileService(fileRepositoryMock.Object, notifyService.Object, Mock.Of<ILogger<FileService>>());

		// Act & Assert
		var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
			await fileService.SaveFileAsync(userId, fileName, fileStream, cancellationToken));

		Assert.Equal($"[{nameof(FileService)}] - Unexpected error while saving file. User: {userId}, File: {fileName}", exception.Message);
		fileRepositoryMock.Verify(repo => repo.UpdateAsync(userId, fileStream, fileName, cancellationToken), Times.Once);
	}

}
