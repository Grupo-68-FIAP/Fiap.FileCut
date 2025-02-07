using Fiap.FileCut.Core.Interfaces.Services;
using System.IO.Compression;

namespace Fiap.FileCut.Processing.Services
{
    public class PackageService : IPackageService
    {
        public async Task<string> PackageImagesAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "The directory path cannot be null or empty.");
            }

            string zipFilePath = Path.Combine(filePath, "images.zip");

            await Task.Run(() =>
            {
                if (Directory.Exists(filePath))
                {
                    ZipFile.CreateFromDirectory(filePath, zipFilePath);
                }
                else
                {
                    throw new DirectoryNotFoundException($"The directory '{filePath}' does not exist.");
                }
            });

            return zipFilePath;
        }
    }
}