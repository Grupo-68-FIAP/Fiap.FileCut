using Fiap.FileCut.Core.Interfaces.Services;
using FFMpegCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Drawing;
using System.IO.Compression;
using Fiap.FileCut.Processing.Exceptions;

namespace Fiap.FileCut.Processing.Services;

public class VideoProcessingService : IVideoProcessingService
{
    private readonly ILogger<VideoProcessingService> _logger;
    private readonly ProcessingOptions _options;

    public VideoProcessingService(
        ILogger<VideoProcessingService> logger,
        IOptions<ProcessingOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task ProcessVideoAsync(
        string videoPath, 
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Gerar identificador único para o processamento
            var processingId = DateTime.Now.ToString("yyyyMMddHHmmss");

            // Criar estrutura de diretórios
            var processingFolder = Path.Combine(
                _options.WorkingDirectory, 
                userId.ToString(), 
                $"processing_{processingId}");

            // Configuração de Diretório
            var outputFolder = Path.Combine(processingFolder, "frames");
            Directory.CreateDirectory(outputFolder);

            // Análise do Vídeo
            var analysisOptions = new FFOptions { 
                UseCache = true,
                TemporaryFilesFolder = _options.WorkingDirectory
            };

            var videoInfo = await FFProbe.AnalyseAsync(
                videoPath, 
                analysisOptions, 
                cancellationToken);

            // Parâmetros de Processamento
            var duration = videoInfo.Duration;
            var interval = TimeSpan.FromSeconds(_options.FrameIntervalSeconds);

            // Extração de Frames
            for (var currentTime = TimeSpan.Zero; currentTime < duration; currentTime += interval)
            {
                // Geração do Frame
                var outputPath = Path.Combine(outputFolder, $"frame_{currentTime.TotalSeconds}.jpg");
                await FFMpeg.SnapshotAsync(
                    videoPath, 
                    outputPath, 
                    new Size(_options.FrameWidth, _options.FrameHeight), 
                    currentTime);
            }


            // Criação do ZIP
           var zipFileName = _options.ZipFileNameFormat
            .Replace("{userId}", userId.ToString())
            .Replace("{timestamp}", processingId)
            .Replace("{guid}", Guid.NewGuid().ToString());

            var zipFilePath = Path.Combine(processingFolder, $"{zipFileName}.zip");
            ZipFile.CreateFromDirectory(outputFolder, zipFilePath);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante o processamento do vídeo para o usuário {UserId}", userId);
            throw new VideoProcessingException("Falha crítica no processamento de vídeo", ex);
        }
    }
}