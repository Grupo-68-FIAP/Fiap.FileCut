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
			var errorMessage = $"[{nameof(FileService)}] - Error deleting file. User: {userId}, File: {fileName}";
			_logger.LogError(ex, errorMessage);
			throw new ApplicationException(errorMessage, ex);
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
			var errorMessage = $"[{nameof(FileService)}] - Error while getting the file. User: {userId}, File: {fileName}";
			_logger.LogError(ex, errorMessage);
			throw new ApplicationException(errorMessage, ex);
		}
	}

	public async Task<IList<string>> GetFileNamesAsync(Guid userId, CancellationToken cancellationToken)
	{
		try
		{
			_logger.LogInformation("[{source}] - Starting file name listing. User: {UserId}", nameof(FileService), userId);

			var files = await _fileRepository.GetAllAsync(userId, cancellationToken);
			var fileNames = files.Select(file => file.FileName).ToList();

			_logger.LogInformation("[{source}] - Successfully listed {FileCount} file names for user {UserId}", nameof(FileService), fileNames.Count, userId);

			return fileNames;
		}
		catch (Exception ex)
		{
			var errorMessage = $"[{nameof(FileService)}] - Error while listing file names. User: {userId}";
			_logger.LogError(ex, errorMessage);
			throw new ApplicationException(errorMessage, ex);
		}
	}

	public async Task<bool> SaveFileAsync(Guid userId, IFormFile file, CancellationToken cancellationToken)
	{
		try
		{
			if (file == null)
				throw new ArgumentNullException(nameof(file), "File cannot be null.");

			if (file.Length <= 0)
				throw new ArgumentException("Invalid file size", nameof(file));

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
			_logger.LogWarning(ex, "[{source}] - Validation error while saving file. User: {UserId}, File: {FileName}", nameof(FileService), userId, file?.FileName);
			throw;
		}
		catch (Exception ex)
		{
			var errorMessage = $"[{nameof(FileService)}] - Unexpected error while saving file. User: {userId}, File: {file?.FileName}";
			_logger.LogError(ex, errorMessage);
			throw new ApplicationException(errorMessage, ex);
		}
	}
}