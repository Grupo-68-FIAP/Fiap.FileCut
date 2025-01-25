using Fiap.FileCut.Core.Interfaces.Repository;
using Fiap.FileCut.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Fiap.FileCut.Core.Services;

public class FileService : IFileService
{
	private readonly IFileRepository _fileRepository;
	private readonly ILogger<FileService> _logger;

	public FileService(
		IFileRepository fileRepository,
		ILogger<FileService> logger)
	{
		_fileRepository = fileRepository;
		_logger = logger;
	}

	public async Task<bool> DeleteFileAsync(Guid userId, string fileName, CancellationToken cancellationToken)
	{
		try
		{
			_logger.LogInformation("[{source}] - Starting file deletion. User: {UserId}, File: {FileName}", nameof(FileService), userId, fileName);

			var result = await _fileRepository.DeleteAsync(userId, fileName, cancellationToken);

			if (result)
				_logger.LogInformation("[{source}] - File deleted successfully. User: {UserId}, File: {FileName}", nameof(FileService), userId, fileName);
			else
				_logger.LogWarning("[{source}] - Failed to delete file. User: {UserId}, File: {FileName}", nameof(FileService), userId, fileName);

			return result;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "[{source}] - Unexpected error while deleting file. User: {UserId}, File: {FileName}", nameof(FileService), userId, fileName);
			throw;
		}
	}

	public async Task<IFormFile> GetFileAsync(Guid userId, string fileName, CancellationToken cancellationToken)
	{
		try
		{
			_logger.LogInformation("[{source}] - Starting file download. User: {UserId}, File: {FileName}", nameof(FileService), userId, fileName);

			return await _fileRepository.GetAsync(userId, fileName, cancellationToken);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "[{source}] - Error while getting the file. User: {UserId}, File: {FileName}", nameof(FileService), userId, fileName);
			throw;
		}
	}

	public async Task<bool> SaveFileAsync(Guid userId, IFormFile file, CancellationToken cancellationToken)
	{
		try
		{
			if (file.Length <= 0)
				throw new ArgumentException("Invalid file size");

			_logger.LogInformation("[{source}] - Starting file upload. User: {UserId}, File: {FileName}", nameof(FileService), userId, file.FileName);

			var result = await _fileRepository.UpdateAsync(userId, file, cancellationToken);
			if (result)
				_logger.LogInformation("[{source}] - File saved successfully. User: {UserId}, File: {FileName}", nameof(FileService), userId, file.FileName);
			else
				_logger.LogWarning("[{source}] - Failed to save file. User: {UserId}, File: {FileName}", nameof(FileService), userId, file.FileName);

			return result;
		}
		catch (ArgumentException ex)
		{
			_logger.LogError(ex, "[{source}] - Validation error while saving file. User: {UserId}, File: {FileName}", nameof(FileService), userId, file.FileName);
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "[{source}] - Unexpected error while saving file. User: {UserId}, File: {FileName}", nameof(FileService), userId, file.FileName);
			throw;
		}
	}
}