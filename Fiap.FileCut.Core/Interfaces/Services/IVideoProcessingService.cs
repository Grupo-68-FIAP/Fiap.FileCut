using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiap.FileCut.Core.Interfaces.Services
{
    public interface IVideoProcessingService
    {
        Task ProcessarVideoAsync(string videoPath, string outputFolder, CancellationToken cancellationToken = default);
        Task<byte[]> GerarThumbnailsZipAsync(string videoPath, TimeSpan interval, CancellationToken cancellationToken = default);
    }
}
