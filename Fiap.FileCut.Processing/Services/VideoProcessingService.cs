using FFMpegCore;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Fiap.FileCut.Processing.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Drawing;
using System.IO.Compression;

namespace Fiap.FileCut.Processing.Services;

public class VideoProcessingService(
    ILogger<VideoProcessingService> logger,
    IOptions<ProcessingOptions> options) : IVideoProcessingService
{
    private readonly ProcessingOptions _options = options.Value;

    public async Task<Stream> ProcessVideoAsync(Stream video, CancellationToken cancellationToken = default)
    {
        var processId = Guid.NewGuid();
        var basePath = Path.Combine(Path.GetTempPath(), processId.ToString());
        Directory.CreateDirectory(basePath);

        try
        {
            logger.LogDebug("Iniciando processamento de vídeo");
            var videoName = $"{processId}.mp4";

            // Salvar o arquivo de vídeo
            var localVideo = Path.Combine(basePath, videoName);
            video.Seek(0, SeekOrigin.Begin);
            using (var fileStream = new FileStream(localVideo, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await video.CopyToAsync(fileStream, cancellationToken);
            }

            // Parâmetros de Processamento
            var videoInfo = await FFProbe.AnalyseAsync(localVideo, cancellationToken: cancellationToken);
            var duration = videoInfo.Duration;
            var interval = TimeSpan.FromSeconds(_options.FrameIntervalSeconds);

            // Extração de Frames
            for (var currentTime = TimeSpan.Zero; currentTime < duration; currentTime += interval)
            {
                cancellationToken.ThrowIfCancellationRequested();
                // Geração do Frame
                var tmpPath = Path.Combine(basePath, $"frame_{currentTime.TotalSeconds}.jpg");
                var size = new Size(_options.FrameWidth, _options.FrameHeight);
                await FFMpeg.SnapshotAsync(localVideo, tmpPath, size, currentTime);
            }
            File.Delete(localVideo);

            // Compactação
            Stream zip = new MemoryStream();
            ZipFile.CreateFromDirectory(basePath, zip);
            return zip;
        }
        catch (Exception ex)
        {
            throw new VideoProcessingException("Falha crítica no processamento de vídeo", ex);
        }
        finally
        {
            Directory.Delete(basePath, true);
        }
    }
}