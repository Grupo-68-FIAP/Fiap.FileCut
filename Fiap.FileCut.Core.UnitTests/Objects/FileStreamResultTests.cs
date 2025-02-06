using Fiap.FileCut.Infra.Storage.Shared.Models;

namespace Fiap.FileCut.Core.UnitTests.Objects
{
	public class FileStreamResultTests
	{
		[Fact]
		public void Dispose_ShouldDisposeFileStream_WhenCalled()
		{
			// Arrange
			var memoryStream = new MemoryStream();
			var fileStreamResult = new FileStreamResult("test.txt", memoryStream);

			// Act
			fileStreamResult.Dispose();

			// Assert
			Assert.Throws<ObjectDisposedException>(() => memoryStream.WriteByte(0));
		}

		[Fact]
		public void Dispose_ShouldNotThrow_WhenCalledMultipleTimes()
		{
			// Arrange
			var memoryStream = new MemoryStream();
			var fileStreamResult = new FileStreamResult("test.txt", memoryStream);

			// Act
			fileStreamResult.Dispose(); 
			fileStreamResult.Dispose();

			// Assert
			Assert.True(true);
		}
	}
}
