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
			_logger.LogInformation("[{Source}] - Starting file deletion. User: {UserId}, File: {FileName}", nameof(FileService), userId, fileName);

			var result = await _fileRepository.DeleteAsync(userId, fileName, cancellationToken);

			if (result)
				_logger.LogInformation("[{Source}] - File deleted successfully. User: {UserId}, File: {FileName}", nameof(FileService), userId, fileName);
			else
				_logger.LogWarning("[{Source}] - Failed to delete file. User: {UserId}, File: {FileName}", nameof(FileService), userId, fileName);

			return result;
		}
		catch (Exception ex)
		{
			const string errorMessageTemplate = "[{ClassName}] - Error deleting file. User: {UserId}, File: {FileName}";
			var errorMessage = string.Format(errorMessageTemplate, nameof(FileService), userId, fileName);
			_logger.LogError(ex, errorMessage);
			throw new InvalidOperationException(errorMessage, ex);
		}
	}

	public async Task<IFormFile> GetFileAsync(Guid userId, string fileName, CancellationToken cancellationToken)
	{
		try
		{
			const string infoMessageTemplate = "[{Source}] - Starting file download. User: {UserId}, File: {FileName}";
			_logger.LogInformation(infoMessageTemplate, nameof(FileService), userId, fileName);

			return await _fileRepository.GetAsync(userId, fileName, cancellationToken);
		}
		catch (Exception ex)
		{
			const string errorMessageTemplate = "[{Source}] - Error while getting the file. User: {UserId}, File: {FileName}";
			var errorMessage = string.Format(errorMessageTemplate, nameof(FileService), userId, fileName);
			_logger.LogError(ex, errorMessage);
			throw new InvalidOperationException(errorMessage, ex);
		}
	}

	public async Task<IList<string>> GetFileNamesAsync(Guid userId, CancellationToken cancellationToken)
	{
		try
		{
			const string infoMessageTemplate = "[{Source}] - Starting file name listing. User: {UserId}";
			_logger.LogInformation(infoMessageTemplate, nameof(FileService), userId);

			var files = await _fileRepository.GetAllAsync(userId, cancellationToken);
			var fileNames = files.Select(file => file.FileName).ToList();

			const string successMessageTemplate = "[{Source}] - Successfully listed {FileCount} file names for user {UserId}";
			_logger.LogInformation(successMessageTemplate, nameof(FileService), fileNames.Count, userId);

			return fileNames;
		}
		catch (Exception ex)
		{
			const string errorMessageTemplate = "[{Source}] - Error while listing file names. User: {UserId}";
			var errorMessage = string.Format(errorMessageTemplate, nameof(FileService), userId);
			_logger.LogError(ex, errorMessage);
			throw new InvalidOperationException(errorMessage, ex); 
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

			const string infoMessageTemplate = "[{Source}] - Starting file upload. User: {UserId}, File: {FileName}";
			_logger.LogInformation(infoMessageTemplate, nameof(FileService), userId, file.FileName);

			var result = await _fileRepository.UpdateAsync(userId, file, cancellationToken);
			if (result)
			{
				const string successMessageTemplate = "[{Source}] - File saved successfully. User: {UserId}, File: {FileName}";
				_logger.LogInformation(successMessageTemplate, nameof(FileService), userId, file.FileName);
			}
			else
			{
				const string warningMessageTemplate = "[{Source}] - Failed to save file. User: {UserId}, File: {FileName}";
				_logger.LogWarning(warningMessageTemplate, nameof(FileService), userId, file.FileName);
			}

			return result;
		}
		catch (ArgumentException ex)
		{
			const string warningMessageTemplate = "[{Source}] - Validation error while saving file. User: {UserId}, File: {FileName}";
			_logger.LogWarning(ex, warningMessageTemplate, nameof(FileService), userId, file.FileName);
			throw new InvalidOperationException("Validation error occurred while saving the file.", ex);
		}
		catch (Exception ex)
		{
			const string errorMessageTemplate = "[{Source}] - Unexpected error while saving file. User: {UserId}, File: {FileName}";
			var errorMessage = string.Format(errorMessageTemplate, nameof(FileService), userId, file.FileName);
			_logger.LogError(ex, errorMessage);
			throw new InvalidOperationException(errorMessage, ex); 
		}
	}
}