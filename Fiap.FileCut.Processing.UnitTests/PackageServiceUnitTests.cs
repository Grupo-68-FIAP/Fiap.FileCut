using Fiap.FileCut.Processing.Services;
using System;
using System.IO;
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
            var testFilePath = Path.Combine(testDirectory, "images.zip");

            Directory.CreateDirectory(testDirectory);
            await File.WriteAllTextAsync(Path.Combine(testDirectory, "test.txt"), "test content");

            // Act
            var result = await packageService.PackageImagesAsync(testDirectory);

            // Assert
            Assert.True(File.Exists(testFilePath));
            Assert.Equal(testFilePath, result);

            // Cleanup
            Directory.Delete(testDirectory, true);
        }

        [Fact]
        public async Task PackageImagesAsync_Should_Throw_ArgumentNullException_When_DirectoryPath_Is_Null()
        {
            // Arrange
            var packageService = new PackageService();
            string? invalidFilePath = null;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => packageService.PackageImagesAsync(invalidFilePath!));
            Assert.Equal("The directory path cannot be null or empty. (Parameter 'filePath')", exception.Message);
        }

        [Fact]
        public async Task PackageImagesAsync_Should_Throw_DirectoryNotFoundException_When_Directory_Does_Not_Exist()
        {
            // Arrange
            var packageService = new PackageService();
            var nonExistentDirectory = "non_existent_directory";

            // Act & Assert
            await Assert.ThrowsAsync<DirectoryNotFoundException>(() => packageService.PackageImagesAsync(nonExistentDirectory));
        }

        [Fact]
        public async Task PackageImagesAsync_Should_Return_ZipFilePath()
        {
            // Arrange
            var packageService = new PackageService();
            var testDirectory = "test_directory";
            var testFilePath = Path.Combine(testDirectory, "images.zip");

            Directory.CreateDirectory(testDirectory);
            await File.WriteAllTextAsync(Path.Combine(testDirectory, "test.txt"), "test content");

            // Act
            var result = await packageService.PackageImagesAsync(testDirectory);

            // Assert
            Assert.Equal(testFilePath, result);

            // Cleanup
            Directory.Delete(testDirectory, true);
        }
    }
}