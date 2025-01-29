using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiap.FileCut.Core.Services
{
    public class VideoProcessingService : IVideoProcessingService
    {
        private readonly ILogger<VideoProcessingService> _logger;
        private readonly VideoProcessingSettings _settings;

        public VideoProcessingService(
            IOptions<VideoProcessingSettings> settings,
            ILogger<VideoProcessingService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task ProcessarVideoAsync(string videoPath, string outputFolder, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Iniciando processamento do vídeo: {VideoPath}", videoPath);

                var videoInfo = await FFProbe.AnalyseAsync(videoPath, cancellationToken);
                var duration = videoInfo.Duration;

                Directory.CreateDirectory(outputFolder);

                for (var currentTime = TimeSpan.Zero; currentTime < duration; currentTime += _settings.IntervaloCaptura)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await ProcessarFrameAsync(videoPath, outputFolder, currentTime);
                }

                _logger.LogInformation("Processamento concluído com sucesso");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Processamento cancelado");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante o processamento do vídeo");
                throw new VideoProcessingException("Falha no processamento do vídeo", ex);
            }
        }

        public async Task<byte[]> GerarThumbnailsZipAsync(string videoPath, TimeSpan interval, CancellationToken cancellationToken = default)
        {
            var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            try
            {
                Directory.CreateDirectory(tempFolder);
                await ProcessarVideoAsync(videoPath, tempFolder, cancellationToken);

                var zipPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.zip");
                ZipFile.CreateFromDirectory(tempFolder, zipPath);

                return await File.ReadAllBytesAsync(zipPath, cancellationToken);
            }
            finally
            {
                CleanupTempFiles(tempFolder);
            }
        }

        private async Task ProcessarFrameAsync(string videoPath, string outputFolder, TimeSpan currentTime)
        {
            var outputFileName = $"frame_at_{currentTime.TotalSeconds:000}.jpg";
            var outputPath = Path.Combine(outputFolder, outputFileName);

            _logger.LogDebug("Gerando frame: {CurrentTime}", currentTime);

            await FFMpeg.SnapshotAsync(
                videoPath,
                outputPath,
                _settings.TamanhoFrame,
                currentTime);
        }

        private void CleanupTempFiles(string tempFolder)
        {
            try
            {
                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao limpar arquivos temporários");
            }
        }
    }
}
