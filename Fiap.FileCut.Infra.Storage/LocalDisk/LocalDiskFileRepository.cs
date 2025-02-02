using Fiap.FileCut.Core.Interfaces.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace Fiap.FileCut.Infra.Storage.LocalDisk;

public class LocalDiskFileRepository : IFileRepository
{
	private readonly string _localStorageFolderPath;

	public LocalDiskFileRepository()
	{
		_localStorageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "LocalStorage");
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
								 .ToList();

		return await Task.FromResult(fileNames);
	}

	public async Task<IList<IFormFile>> GetAllAsync(Guid userId, CancellationToken cancellationToken)
	{
		try
		{
			string userFolderPath = Path.Combine(_localStorageFolderPath, userId.ToString());
			if (!Directory.Exists(userFolderPath))
				return new List<IFormFile>(); 

			var files = Directory.GetFiles(userFolderPath);
			var formFiles = new List<IFormFile>();

			foreach (var filePath in files)
			{
				var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
				var fileName = Path.GetFileName(filePath);

				var formFile = new FormFile(fileStream, 0, fileStream.Length, fileName, fileName);
				formFiles.Add(formFile);
			}

			return await Task.FromResult(formFiles);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Failed to retrieve files for user {userId}.", ex);
		}
	}

	public async Task<IFormFile?> GetAsync(Guid userId, string fileName, CancellationToken cancellationToken)
	{
		try
		{
			string userFolderPath = Path.Combine(_localStorageFolderPath, userId.ToString());
			string filePath = Path.Combine(userFolderPath, fileName);

			if (!File.Exists(filePath))
				return null;

			var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			return await Task.FromResult(new FormFile(fileStream, 0, fileStream.Length, fileName, fileName));
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Failed to retrieve file {fileName} for user {userId}.", ex);
		}
	}

	public async Task<bool> UpdateAsync(Guid userId, IFormFile file, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(file);

		try
		{
			if (file.Length <= 0)
				return false;

			string userFolderPath = Path.Combine(_localStorageFolderPath, userId.ToString());
			EnsureDirectoryExists(userFolderPath);

			string filePath = Path.Combine(userFolderPath, file.FileName);

			using (var fileStream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(fileStream, cancellationToken);
			}

			return File.Exists(filePath);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Failed to save file {file.FileName} for user {userId}.", ex);
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