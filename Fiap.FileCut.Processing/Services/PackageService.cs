using Fiap.FileCut.Core.Interfaces.Services;
using System.IO.Compression;

namespace Fiap.FileCut.Processing.Services
{
    public class PackageService : IPackageService
    {
        public async Task<string> PackageImagesAsync(string filePath)
        {
            await Task.Run(() =>
            {
                string outputDirectory = Path.GetDirectoryName(filePath) ?? throw new ArgumentNullException(nameof(filePath), "The directory name cannot be null.");
                ZipFile.CreateFromDirectory(outputDirectory, filePath);
            });

            return filePath;
        }
    }
}
