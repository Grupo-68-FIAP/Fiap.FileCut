using Fiap.FileCut.Core.Interfaces.Repository;
using Fiap.FileCut.Infra.Storage.Shared.Models;

namespace Fiap.FileCut.Infra.Storage.LocalDisk;

public class LocalDiskFileRepository : IFileRepository
{
	private readonly string _localStorageFolderPath;

	public LocalDiskFileRepository()
	{
		_localStorageFolderPath = Path.Combine(Path.GetTempPath(), "Fiap-FileCute-LocalStorage");
		EnsureDirectoryExists(_localStorageFolderPath);
	}

	public async Task<bool> DeleteAsync(Guid userId, string fileName, CancellationToken cancellationToken)
	{
		try
		{
			string userFolderPath = Path.Combine(_localStorageFolderPath, userId.ToString());
			string filePath = Path.Combine(userFolderPath, fileName);

			if (!File.Exists(filePath))
				return false;

			await Task.Run(() => File.Delete(filePath), cancellationToken);

			return !File.Exists(filePath);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Failed to delete file {fileName} for user {userId}.", ex);
		}
	}

	public async Task<IList<string>> ListFileNamesAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		if (userId == Guid.Empty)
			throw new ArgumentException("User ID cannot be empty.", nameof(userId));

		string userFolderPath = Path.Combine(_localStorageFolderPath, userId.ToString());

		if (!Directory.Exists(userFolderPath))
		{
			return new List<string>();
		}

		var fileNames = Directory.GetFiles(userFolderPath)
								 .Select(Path.GetFileName)
								 .Where(name => !string.IsNullOrEmpty(name))
								 .Select(name => name!) 
								 .ToList();

		return await Task.FromResult<IList<string>>(fileNames);
	}

	public async Task<IList<FileStreamResult>> GetAllAsync(Guid userId, CancellationToken cancellationToken)
	{
		try
		{
			string userFolderPath = Path.Combine(_localStorageFolderPath, userId.ToString());
			if (!Directory.Exists(userFolderPath))
				return new List<FileStreamResult>();

			var files = Directory.GetFiles(userFolderPath);
			var fileResults = new List<FileStreamResult>();

			foreach (var filePath in files)
			{
				var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
				var fileName = Path.GetFileName(filePath);

				fileResults.Add(new FileStreamResult(fileName, fileStream));
			}

			return await Task.FromResult(fileResults);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Failed to retrieve files for user {userId}.", ex);
		}
	}

	public async Task<FileStreamResult?> GetAsync(Guid userId, string fileName, CancellationToken cancellationToken)
	{
		try
		{
			string userFolderPath = Path.Combine(_localStorageFolderPath, userId.ToString());
			string filePath = Path.Combine(userFolderPath, fileName);

			if (!File.Exists(filePath))
				return null;

			var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			return await Task.FromResult(new FileStreamResult(fileName, fileStream));
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Failed to retrieve file {fileName} for user {userId}.", ex);
		}
	}

	public async Task<bool> UpdateAsync(Guid userId, Stream fileStream, string fileName, CancellationToken cancellationToken)
	{
		try
		{
			if (fileStream == null || fileStream.Length == 0)
				return false;

			string userFolderPath = Path.Combine(_localStorageFolderPath, userId.ToString());
			EnsureDirectoryExists(userFolderPath);

			string filePath = Path.Combine(userFolderPath, fileName);

			using (var outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				await fileStream.CopyToAsync(outputStream, cancellationToken);
			}

			return File.Exists(filePath);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Failed to save file {fileName} for user {userId}.", ex);
		}
	}

	private static void EnsureDirectoryExists(string path)
	{
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
	}
}