using Amazon.S3;
using Amazon.S3.Model;
using Fiap.FileCut.Infra.Storage.S3;
using Fiap.FileCut.Infra.Storage.Shared.Exceptions;
using Fiap.FileCut.Infra.Storage.Shared.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fiap.FileCut.Core.InfraStorage.UnitTests.S3
{
	public class S3FileRepositoryUnitTests
	{
		private readonly Mock<IAmazonS3> _s3ClientMock;
		private readonly S3FileRepository _fileRepository;

		public S3FileRepositoryUnitTests()
		{ 
			_s3ClientMock = new Mock<IAmazonS3>(); 
			var s3Helper = new S3Helper(_s3ClientMock.Object, "my-bucket-name"); 
			_fileRepository = new S3FileRepository(_s3ClientMock.Object, "my-bucket-name", Mock.Of<ILogger<S3FileRepository>>(), s3Helper);
		}

		[Fact]
		public async Task GetAsync_WhenFileFound_ShouldReturnFile()
		{
			var userId = Guid.NewGuid();
			var fileName = "testfile.txt";
			var cancellationToken = CancellationToken.None;

			var fileBytes = new byte[] { 1, 2, 3 };

			var s3ObjectMock = new GetObjectResponse
			{
				ResponseStream = new MemoryStream(fileBytes)
			};

			_s3ClientMock.Setup(s3Client => s3Client.GetObjectAsync(It.IsAny<GetObjectRequest>(), cancellationToken))
				.ReturnsAsync(s3ObjectMock);

			var result = await _fileRepository.GetAsync(userId, fileName, cancellationToken);

			Assert.NotNull(result);
			Assert.Equal(fileBytes.Length, result.Length);
			using (var memoryStream = new MemoryStream())
			{
				await result.CopyToAsync(memoryStream);
				Assert.Equal(fileBytes, memoryStream.ToArray());
			}

			_s3ClientMock.Verify(s3Client => s3Client.GetObjectAsync(It.IsAny<GetObjectRequest>(), cancellationToken), Times.Once);
		}

		[Fact]
		public async Task GetAsync_WhenS3ObjectIsNull_ShouldThrowInvalidOperationException()
		{
			var userId = Guid.NewGuid();
			var fileName = "testfile.txt";
			var cancellationToken = CancellationToken.None;

			var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
				_fileRepository.GetAsync(userId, fileName, cancellationToken));

			Assert.Equal($"Unexpected error while downloading file '{fileName}' for user '{userId}'.", exception.Message);

			_s3ClientMock.Verify(s3Client => s3Client.GetObjectAsync(It.IsAny<GetObjectRequest>(), cancellationToken), Times.Once);
		}

		[Fact]
		public async Task GetAsync_WhenResponseStreamIsNull_ShouldThrowInvalidOperationException()
		{
			var userId = Guid.NewGuid();
			var fileName = "testfile.txt";
			var cancellationToken = CancellationToken.None;

			var s3ObjectMock = new GetObjectResponse
			{
				ResponseStream = null
			};

			_s3ClientMock.Setup(s3Client => s3Client.GetObjectAsync(It.IsAny<GetObjectRequest>(), cancellationToken))
				.ReturnsAsync(s3ObjectMock);

			var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
				_fileRepository.GetAsync(userId, fileName, cancellationToken));

			Assert.Equal($"Unexpected error while downloading file '{fileName}' for user '{userId}'.", exception.Message);

			_s3ClientMock.Verify(s3Client => s3Client.GetObjectAsync(It.IsAny<GetObjectRequest>(), cancellationToken), Times.Once);
		}

		[Fact]
		public async Task GetAsync_WhenFileNotFound_ShouldThrowFileRepositoryException()
		{
			var userId = Guid.NewGuid();
			var fileName = "testfile.txt";
			var cancellationToken = CancellationToken.None;

			_s3ClientMock.Setup(s3Client => s3Client.GetObjectAsync(It.IsAny<GetObjectRequest>(), cancellationToken))
				.ThrowsAsync(new AmazonS3Exception("The specified key does not exist"));

			var exception = await Assert.ThrowsAsync<FileRepositoryException>(() => _fileRepository.GetAsync(userId, fileName, cancellationToken));
			Assert.Contains("Error downloading file", exception.Message);
		}

		[Fact]
		public async Task GetAllAsync_WhenNoFilesExist_ShouldReturnEmptyList()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var cancellationToken = CancellationToken.None;

			var listResponse = new ListObjectsV2Response
			{
				S3Objects = new List<S3Object>()
			};

			_s3ClientMock.Setup(client => client.ListObjectsV2Async(It.IsAny<ListObjectsV2Request>(), cancellationToken))
				.ReturnsAsync(listResponse);

			// Act
			var result = await _fileRepository.GetAllAsync(userId, cancellationToken);

			// Assert
			Assert.Empty(result);
			_s3ClientMock.Verify(client => client.ListObjectsV2Async(It.IsAny<ListObjectsV2Request>(), cancellationToken), Times.Once);
		}

		[Fact]
		public async Task GetAsync_WhenInvalidFileName_ShouldThrowInvalidOperationException()
		{
			var userId = Guid.NewGuid();
			var invalidFileName = "invalid<>file.txt";
			var cancellationToken = CancellationToken.None;

			var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _fileRepository.GetAsync(userId, invalidFileName, cancellationToken));
			Assert.Contains("Unexpected error while downloading file", exception.Message);
		}

		[Fact]
		public async Task UpdateAsync_WhenInvalidFileName_ShouldThrowInvalidOperationException()
		{
			var userId = Guid.NewGuid();
			var fileMock = new Mock<IFormFile>();
			fileMock.Setup(f => f.FileName).Returns("invalid|filename.txt");
			var cancellationToken = CancellationToken.None;

			var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _fileRepository.UpdateAsync(userId, fileMock.Object, cancellationToken));
			Assert.Contains("Unexpected error while uploading/updating file", exception.Message);
		}

		[Fact]
		public async Task GetAsync_WhenAmazonS3ErrorOccurs_ShouldThrowFileRepositoryException()
		{
			var userId = Guid.NewGuid();
			var fileName = "testfile.txt";
			var cancellationToken = CancellationToken.None;

			_s3ClientMock.Setup(s3Client => s3Client.GetObjectAsync(It.IsAny<GetObjectRequest>(), cancellationToken))
				.ThrowsAsync(new AmazonS3Exception("S3 Error"));

			var exception = await Assert.ThrowsAsync<FileRepositoryException>(() => _fileRepository.GetAsync(userId, fileName, cancellationToken));
			Assert.Contains("Error downloading file 'testfile.txt' from S3 for user", exception.Message);
		}

		[Fact]
		public async Task UpdateAsync_WhenS3ErrorOccurs_ShouldThrowFileRepositoryException()
		{
			var userId = Guid.NewGuid();
			var fileMock = new Mock<IFormFile>();
			fileMock.Setup(f => f.FileName).Returns("validFileName.txt");
			var cancellationToken = CancellationToken.None;

			_s3ClientMock.Setup(s3Client => s3Client.PutObjectAsync(It.IsAny<PutObjectRequest>(), cancellationToken))
				.ThrowsAsync(new AmazonS3Exception("S3 Error"));

			var exception = await Assert.ThrowsAsync<FileRepositoryException>(() => _fileRepository.UpdateAsync(userId, fileMock.Object, cancellationToken));
			Assert.Contains("AWS S3 error", exception.Message);
		}

		[Fact]
		public async Task DeleteAsync_WhenInvalidFileName_ShouldThrowInvalidOperationException()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var fileName = "invalid|fileName.txt";
			var cancellationToken = CancellationToken.None;

			// Act & Assert
			var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _fileRepository.DeleteAsync(userId, fileName, cancellationToken));
			Assert.Contains("Invalid file name", exception.Message);
		}

		[Fact]
		public async Task DeleteAsync_WhenS3ErrorOccurs_ShouldThrowFileRepositoryException()
		{
			var userId = Guid.NewGuid();
			var fileName = "validFileName.txt";
			var cancellationToken = CancellationToken.None;

			_s3ClientMock.Setup(s3Client => s3Client.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), cancellationToken))
				.ThrowsAsync(new AmazonS3Exception("S3 Error"));

			var exception = await Assert.ThrowsAsync<FileRepositoryException>(() => _fileRepository.DeleteAsync(userId, fileName, cancellationToken));
			Assert.Contains("Error deleting file from S3. Please check", exception.Message);
		}

		[Fact]
		public async Task ListFileNamesAsync_WhenS3ErrorOccurs_ShouldThrowFileRepositoryException()
		{
			var userId = Guid.NewGuid();
			var cancellationToken = CancellationToken.None;

			_s3ClientMock.Setup(s3Client => s3Client.ListObjectsV2Async(It.IsAny<ListObjectsV2Request>(), cancellationToken))
				.ThrowsAsync(new AmazonS3Exception("S3 Error"));

			var exception = await Assert.ThrowsAsync<FileRepositoryException>(() => _fileRepository.ListFileNamesAsync(userId, cancellationToken));
			Assert.Contains("Error listing file names from S3. Please", exception.Message);
		}

		[Fact]
		public async Task GetAllAsync_WhenNoFilesFound_ShouldReturnEmptyList()
		{
			var userId = Guid.NewGuid();
			var cancellationToken = CancellationToken.None;

			var listResponse = new ListObjectsV2Response
			{
				S3Objects = new List<S3Object>()
			};

			_s3ClientMock.Setup(s3Client => s3Client.ListObjectsV2Async(It.IsAny<ListObjectsV2Request>(), cancellationToken))
				.ReturnsAsync(listResponse);

			var result = await _fileRepository.GetAllAsync(userId, cancellationToken);

			Assert.Empty(result);

			_s3ClientMock.Verify(s3Client => s3Client.ListObjectsV2Async(It.IsAny<ListObjectsV2Request>(), cancellationToken), Times.Once);
		}

		[Fact]
		public async Task GetAllAsync_WhenFilesFound_ShouldReturnFileList()
		{
			var userId = Guid.NewGuid();
			var cancellationToken = CancellationToken.None;

			var s3ObjectList = new List<S3Object>
	{
		new S3Object { Key = $"{userId}/testfile1.txt" },
		new S3Object { Key = $"{userId}/testfile2.txt" }
	};

			var listResponse = new ListObjectsV2Response
			{
				S3Objects = s3ObjectList
			};

			_s3ClientMock.Setup(s3Client => s3Client.ListObjectsV2Async(It.IsAny<ListObjectsV2Request>(), cancellationToken))
				.ReturnsAsync(listResponse);

			_s3ClientMock.Setup(client => client.GetObjectAsync(It.IsAny<GetObjectRequest>(), cancellationToken))
				.ReturnsAsync(new GetObjectResponse { ResponseStream = new MemoryStream() });

			var result = await _fileRepository.GetAllAsync(userId, cancellationToken);

			Assert.Equal(2, result.Count);

			_s3ClientMock.Verify(s3Client => s3Client.ListObjectsV2Async(It.IsAny<ListObjectsV2Request>(), cancellationToken), Times.Once);
			_s3ClientMock.Verify(client => client.GetObjectAsync(It.IsAny<GetObjectRequest>(), cancellationToken), Times.Exactly(2));
		}

		[Fact]
		public async Task GetAllAsync_WhenAmazonS3ExceptionThrown_ShouldThrowFileRepositoryException()
		{
			var userId = Guid.NewGuid();
			var cancellationToken = CancellationToken.None;

			_s3ClientMock.Setup(s3Client => s3Client.ListObjectsV2Async(It.IsAny<ListObjectsV2Request>(), cancellationToken))
				.ThrowsAsync(new AmazonS3Exception("S3 error"));

			var exception = await Assert.ThrowsAsync<FileRepositoryException>(() =>
				_fileRepository.GetAllAsync(userId, cancellationToken));

			Assert.Equal($"Error listing files from S3 for user '{userId}'.", exception.Message);

			_s3ClientMock.Verify(s3Client => s3Client.ListObjectsV2Async(It.IsAny<ListObjectsV2Request>(), cancellationToken), Times.Once);
		}

		[Fact]
		public async Task GetAllAsync_WhenExceptionThrown_ShouldThrowInvalidOperationException()
		{
			var userId = Guid.NewGuid();
			var cancellationToken = CancellationToken.None;

			_s3ClientMock.Setup(s3Client => s3Client.ListObjectsV2Async(It.IsAny<ListObjectsV2Request>(), cancellationToken))
				.ThrowsAsync(new Exception("Unexpected error"));

			var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
				_fileRepository.GetAllAsync(userId, cancellationToken));

			Assert.Equal($"Unexpected error while listing files for user '{userId}'.", exception.Message);

			_s3ClientMock.Verify(s3Client => s3Client.ListObjectsV2Async(It.IsAny<ListObjectsV2Request>(), cancellationToken), Times.Once);
		}

		[Fact]
		public async Task DeleteAsync_WhenFileDeletedSuccessfully_ShouldReturnTrue()
		{
			var userId = Guid.NewGuid();
			var fileName = "testfile.txt";
			var cancellationToken = CancellationToken.None;

			var deleteObjectResponse = new DeleteObjectResponse
			{
				HttpStatusCode = System.Net.HttpStatusCode.NoContent
			};

			_s3ClientMock.Setup(s3Client => s3Client.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), cancellationToken))
				.ReturnsAsync(deleteObjectResponse);

			var result = await _fileRepository.DeleteAsync(userId, fileName, cancellationToken);

			Assert.True(result);
			_s3ClientMock.Verify(s3Client => s3Client.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), cancellationToken), Times.Once);
		}

		[Fact]
		public async Task DeleteAsync_WhenUnexpectedErrorOccurs_ShouldThrowFileRepositoryException()
		{
			var userId = Guid.NewGuid();
			var fileName = "testfile.txt";
			var cancellationToken = CancellationToken.None;

			_s3ClientMock.Setup(s3Client => s3Client.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), cancellationToken))
				.ThrowsAsync(new Exception("Unexpected error"));

			var exception = await Assert.ThrowsAsync<FileRepositoryException>(() =>
				_fileRepository.DeleteAsync(userId, fileName, cancellationToken));

			Assert.Equal("An unexpected error occurred while deleting the file. Please try again later.", exception.Message);
			_s3ClientMock.Verify(s3Client => s3Client.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), cancellationToken), Times.Once);
		}

		[Fact]
		public async Task ListFileNamesAsync_WhenNoFilesFound_ShouldReturnEmptyList()
		{
			var userId = Guid.NewGuid();
			var cancellationToken = CancellationToken.None;

			var listObjectsResponse = new ListObjectsV2Response
			{
				S3Objects = new List<S3Object>()
			};

			_s3ClientMock.Setup(s3Client => s3Client.ListObjectsV2Async(It.IsAny<ListObjectsV2Request>(), cancellationToken))
				.ReturnsAsync(listObjectsResponse);

			var result = await _fileRepository.ListFileNamesAsync(userId, cancellationToken);

			Assert.Empty(result);
			_s3ClientMock.Verify(s3Client => s3Client.ListObjectsV2Async(It.IsAny<ListObjectsV2Request>(), cancellationToken), Times.Once);
		}

		[Fact]
		public async Task ListFileNamesAsync_WhenUnexpectedErrorOccurs_ShouldThrowInvalidOperationException()
		{
			var userId = Guid.NewGuid();
			var cancellationToken = CancellationToken.None;

			_s3ClientMock.Setup(s3Client => s3Client.ListObjectsV2Async(It.IsAny<ListObjectsV2Request>(), cancellationToken))
				.ThrowsAsync(new Exception("Unexpected error"));

			var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
				_fileRepository.ListFileNamesAsync(userId, cancellationToken));

			Assert.Equal($"Unexpected error while listing files for user '{userId}'.", exception.Message);
			_s3ClientMock.Verify(s3Client => s3Client.ListObjectsV2Async(It.IsAny<ListObjectsV2Request>(), cancellationToken), Times.Once);
		}
	}
}
