using Fiap.FileCut.Core.Interfaces.Handlers;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FFMpegCore;
using System.Drawing;
using System.IO.Compression;

namespace Fiap.FileCut.Core.Handlers;

public class VideoProcessingHandler(
    INotifyService notifyService,
    ILogger<VideoProcessingHandler> logger,
    IOptions<ProcessingOptions> options) : IMessageHandler<string>
{
    private readonly ILogger<VideoProcessingHandler> _logger = logger;
    private readonly INotifyService _notifyService = notifyService;
    private readonly ProcessingOptions _options = options.Value;

    public async Task HandleAsync(NotifyContext<string> context)
    {
        try
        {
            var videoPath = context.Context;
            var userId = context.UserId;

            _logger.LogInformation("Iniciando processamento do vídeo: {VideoPath} para o usuário {UserId}", 
                videoPath, userId);

            await UpdateStatus(userId, "PROCESSING", "Iniciando processamento");

            var outputFolder = Path.Combine(_options.WorkingDirectory, userId.ToString(), "frames");
            Directory.CreateDirectory(outputFolder);

            var videoInfo = await FFProbe.AnalyseAsync(videoPath);
            var duration = videoInfo.Duration;
            var interval = TimeSpan.FromSeconds(_options.FrameIntervalSeconds);

            await ProcessVideoFrames(videoPath, outputFolder, duration, interval, userId);

            var zipFilePath = Path.Combine(_options.WorkingDirectory, userId.ToString(), "frames.zip");
            ZipFile.CreateFromDirectory(outputFolder, zipFilePath);

            await UpdateStatus(userId, "COMPLETED", "Processamento concluído com sucesso");
            
            _logger.LogInformation("Processamento finalizado para o usuário {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante o processamento do vídeo para o usuário {UserId}", context.UserId);
            await UpdateStatus(context.UserId, "FAILED", ex.Message);
        }
    }

    private async Task ProcessVideoFrames(string videoPath, string outputFolder, TimeSpan duration, 
        TimeSpan interval, Guid userId)
    {
        for (var currentTime = TimeSpan.Zero; currentTime < duration; currentTime += interval)
        {
            _logger.LogDebug("Processando frame: {CurrentTime} para o usuário {UserId}", currentTime, userId);
            
            var outputPath = Path.Combine(outputFolder, $"frame_{currentTime.TotalSeconds}.jpg");
            await FFMpeg.SnapshotAsync(videoPath, outputPath, new Size(_options.FrameWidth, _options.FrameHeight), currentTime);
            
            await UpdateStatus(userId, "PROCESSING", $"Processando frame {currentTime}");
        }
    }

    private async Task UpdateStatus(Guid userId, string status, string message)
    {
        var statusUpdate = new ProcessingStatus {
            UserId = userId,
            Status = status,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        await _notifyService.NotifyAsync(new NotifyContext<ProcessingStatus>(statusUpdate, userId));
    }
}