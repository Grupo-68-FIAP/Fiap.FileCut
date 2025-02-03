using Fiap.FileCut.Infra.Storage.LocalDisk;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiap.FileCut.Core.InfraStorage.UnitTests.LocalDisk
{
	public class LocalDiskFileRepositoryUnitTests
	{
		private readonly LocalDiskFileRepository _fileRepository;

		public LocalDiskFileRepositoryUnitTests()
		{
			_fileRepository = new LocalDiskFileRepository();
		}

		[Fact]
		public async Task DeleteAsync_WhenFileExists_ShouldReturnTrue()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var fileName = "testfile.txt";
			var cancellationToken = CancellationToken.None;
			var userFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "LocalStorage", userId.ToString());

			Directory.CreateDirectory(userFolderPath);
			File.WriteAllText(Path.Combine(userFolderPath, fileName), "test content");

			// Act
			var result = await _fileRepository.DeleteAsync(userId, fileName, cancellationToken);

			// Assert
			Assert.True(result);
			Assert.False(File.Exists(Path.Combine(userFolderPath, fileName)));
		}

		[Fact]
		public async Task DeleteAsync_WhenFileDoesNotExist_ShouldReturnFalse()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var fileName = "nonexistentfile.txt";
			var cancellationToken = CancellationToken.None;

			// Act
			var result = await _fileRepository.DeleteAsync(userId, fileName, cancellationToken);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task ListFileNamesAsync_WhenFilesExist_ShouldReturnFileNames()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var cancellationToken = CancellationToken.None;
			var userFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "LocalStorage", userId.ToString());

			Directory.CreateDirectory(userFolderPath);
			File.WriteAllText(Path.Combine(userFolderPath, "file1.txt"), "content 1");
			File.WriteAllText(Path.Combine(userFolderPath, "file2.txt"), "content 2");

			// Act
			var result = await _fileRepository.ListFileNamesAsync(userId, cancellationToken);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(2, result.Count);
			Assert.Contains("file1.txt", result);
			Assert.Contains("file2.txt", result);
		}

		[Fact]
		public async Task GetAllAsync_WhenFilesExist_ShouldReturnFileList()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var cancellationToken = CancellationToken.None;
			var userFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "LocalStorage", userId.ToString());

			Directory.CreateDirectory(userFolderPath);
			File.WriteAllText(Path.Combine(userFolderPath, "file1.txt"), "content 1");
			File.WriteAllText(Path.Combine(userFolderPath, "file2.txt"), "content 2");

			// Act
			var result = await _fileRepository.GetAllAsync(userId, cancellationToken);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(2, result.Count);
		}

		[Fact]
		public async Task GetAsync_WhenFileExists_ShouldReturnFile()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var fileName = "testfile.txt";
			var cancellationToken = CancellationToken.None;
			var userFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "LocalStorage", userId.ToString());

			Directory.CreateDirectory(userFolderPath);
			File.WriteAllText(Path.Combine(userFolderPath, fileName), "test content");

			// Act
			var result = await _fileRepository.GetAsync(userId, fileName, cancellationToken);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(fileName, result.FileName);
		}

		[Fact]
		public async Task GetAsync_WhenFileDoesNotExist_ShouldReturnNull()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var fileName = "nonexistentfile.txt";
			var cancellationToken = CancellationToken.None;

			// Act
			var result = await _fileRepository.GetAsync(userId, fileName, cancellationToken);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public async Task UpdateAsync_WhenFileIsValid_ShouldReturnTrue()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var fileMock = new Mock<IFormFile>();
			fileMock.Setup(f => f.FileName).Returns("testfile.txt");
			fileMock.Setup(f => f.Length).Returns(10);
			fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[10]));
			var cancellationToken = CancellationToken.None;
			var userFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "LocalStorage", userId.ToString());

			Directory.CreateDirectory(userFolderPath);

			// Act
			var result = await _fileRepository.UpdateAsync(userId, fileMock.Object, cancellationToken);

			// Assert
			Assert.True(result);
			Assert.True(File.Exists(Path.Combine(userFolderPath, "testfile.txt")));
		}

		[Fact]
		public async Task UpdateAsync_WhenFileIsInvalid_ShouldReturnFalse()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var fileMock = new Mock<IFormFile>();
			fileMock.Setup(f => f.FileName).Returns("testfile.txt");
			fileMock.Setup(f => f.Length).Returns(0);  // Invalid file size
			var cancellationToken = CancellationToken.None;
			var userFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "LocalStorage", userId.ToString());

			Directory.CreateDirectory(userFolderPath);

			// Act
			var result = await _fileRepository.UpdateAsync(userId, fileMock.Object, cancellationToken);

			// Assert
			Assert.False(result);
		}
	}
}