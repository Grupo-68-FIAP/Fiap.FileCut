using Fiap.FileCut.Processing.Services;
using Moq;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Xunit;

namespace Fiap.FileCut.Processing.UnitTests
{
    public class PackageServiceUnitTests
    {
        [Fact]
        public async Task PackageImagesAsync_Should_Create_ZipFile()
        {
            // Arrange
            var packageService = new PackageService();
            var testDirectory = "test_directory";
            var testFilePath = Path.Combine(testDirectory, "output.zip");

            Directory.CreateDirectory(testDirectory);
            File.WriteAllText(Path.Combine(testDirectory, "test.txt"), "test content");

            // Act
            var result = await packageService.PackageImagesAsync(testFilePath);

            // Assert
            Assert.True(File.Exists(testFilePath));
            Assert.Equal(testFilePath, result);

            // Cleanup
            Directory.Delete(testDirectory, true);
        }

        [Fact]
        public async Task PackageImagesAsync_Should_Throw_ArgumentNullException_When_DirectoryName_Is_Null()
        {
            // Arrange
            var packageService = new PackageService();
            var invalidFilePath = "invalid_path/output.zip";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => packageService.PackageImagesAsync(invalidFilePath));
            Assert.Equal("The directory name cannot be null. (Parameter 'filePath')", exception.Message);
        }

        [Fact]
        public async Task PackageImagesAsync_Should_Throw_Exception_When_Directory_Does_Not_Exist()
        {
            // Arrange
            var packageService = new PackageService();
            var nonExistentDirectory = "non_existent_directory/output.zip";

            // Act & Assert
            await Assert.ThrowsAsync<DirectoryNotFoundException>(() => packageService.PackageImagesAsync(nonExistentDirectory));
        }
    }
}