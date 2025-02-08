using Fiap.FileCut.Infra.Storage.LocalDisk;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Fiap.FileCut.Core.InfraStorage.UnitTests.LocalDisk
{
	public class LocalDiskFileRepositoryUnitTests
	{
		private readonly string localStorageFolderPath = Path.Combine(Path.GetTempPath(), "Fiap-FileCute-LocalStorage");
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
			var userFolderPath = Path.Combine(localStorageFolderPath, userId.ToString());

			Directory.CreateDirectory(userFolderPath);
			File.WriteAllText(Path.Combine(userFolderPath, fileName), "test content");

			// Act
			var result = await _fileRepository.DeleteAsync(userId, fileName, cancellationToken);

			// Assert
			Assert.True(result);
			Assert.False(File.Exists(Path.Combine(userFolderPath, fileName)));
		}

		[Fact]
		public async Task DeleteAsync_WhenExceptionOccurs_ShouldThrowInvalidOperationException()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var fileName = "testfile.txt";
			var cancellationToken = CancellationToken.None;
			var userFolderPath = Path.Combine(localStorageFolderPath, userId.ToString());
			Directory.CreateDirectory(userFolderPath);

			var filePath = Path.Combine(userFolderPath, fileName);
			File.WriteAllText(filePath, "test content");

			File.SetAttributes(filePath, FileAttributes.ReadOnly);

			// Act & Assert
			var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
			{
				await _fileRepository.DeleteAsync(userId, fileName, cancellationToken);
			});

			Assert.Contains($"Failed to delete file {fileName} for user {userId}.", exception.Message);
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
			var userFolderPath = Path.Combine(localStorageFolderPath, userId.ToString());

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
		public async Task ListFileNamesAsync_WhenUserIdIsEmpty_ShouldThrowArgumentException()
		{
			// Arrange
			var userId = Guid.Empty;
			var cancellationToken = CancellationToken.None;

			// Act & Assert
			var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
			{
				await _fileRepository.ListFileNamesAsync(userId, cancellationToken);
			});

			// Verificar a mensagem de exceção
			Assert.Contains("User ID cannot be empty.", exception.Message);
		}

		[Fact]
		public async Task ListFileNamesAsync_WhenDirectoryDoesNotExist_ShouldReturnEmptyList()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var cancellationToken = CancellationToken.None;
			var userFolderPath = Path.Combine(localStorageFolderPath, userId.ToString());

			if (Directory.Exists(userFolderPath))
			{
				Directory.Delete(userFolderPath, true);
			}

			// Act
			var result = await _fileRepository.ListFileNamesAsync(userId, cancellationToken);

			// Assert
			Assert.NotNull(result);
			Assert.Empty(result); 
		}

		[Fact]
		public async Task GetAllAsync_WhenFilesExist_ShouldReturnFileList()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var cancellationToken = CancellationToken.None;
			var userFolderPath = Path.Combine(localStorageFolderPath, userId.ToString());

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
		public async Task GetAllAsync_WhenDirectoryDoesNotExist_ShouldReturnEmptyList()
		{
			// Arrange
			var userId = Guid.NewGuid(); 
			var cancellationToken = CancellationToken.None;
			var userFolderPath = Path.Combine(localStorageFolderPath, userId.ToString());

			if (Directory.Exists(userFolderPath))
			{
				Directory.Delete(userFolderPath, true);
			}

			// Act
			var result = await _fileRepository.GetAllAsync(userId, cancellationToken);

			// Assert
			Assert.NotNull(result);
			Assert.Empty(result); 
		}

		[Fact]
		public async Task GetAsync_WhenFileExists_ShouldReturnFile()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var fileName = "testfile.txt";
			var cancellationToken = CancellationToken.None;
			var userFolderPath = Path.Combine(localStorageFolderPath, userId.ToString());

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
			var fileName = "testfile.txt";
			var fileStream = new MemoryStream(new byte[10]); 
			var cancellationToken = CancellationToken.None;
			var userFolderPath = Path.Combine(localStorageFolderPath, userId.ToString());

			Directory.CreateDirectory(userFolderPath);

			// Act
			var result = await _fileRepository.UpdateAsync(userId, fileStream, fileName, cancellationToken);

			// Assert
			Assert.True(result);
			Assert.True(File.Exists(Path.Combine(userFolderPath, fileName)));
		}

		[Fact]
		public async Task UpdateAsync_WhenFileIsInvalid_ShouldReturnFalse()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var fileName = "testfile.txt";
			var fileStream = new MemoryStream(); // Stream vazio (simula um arquivo com comprimento 0)
			var cancellationToken = CancellationToken.None;
			var userFolderPath = Path.Combine(localStorageFolderPath, userId.ToString());

			Directory.CreateDirectory(userFolderPath);

			// Act
			var result = await _fileRepository.UpdateAsync(userId, fileStream, fileName, cancellationToken);

			// Assert
			Assert.False(result);
		}
	}
}