using Fiap.FileCut.Core.Interfaces.Services;
using System.IO.Compression;

namespace Fiap.FileCut.Processing.Services
{
    public class PackageService : IPackageService
    {
        public async Task<string> PackageImagesAsync(string zipFilePath)
        {
            await Task.Run(() =>
            {
                string outputDirectory = Path.GetDirectoryName(zipFilePath) ?? throw new ArgumentNullException(nameof(zipFilePath), "The directory name cannot be null.");
                ZipFile.CreateFromDirectory(outputDirectory, zipFilePath);
            });

            return zipFilePath;
        }
    }
}
