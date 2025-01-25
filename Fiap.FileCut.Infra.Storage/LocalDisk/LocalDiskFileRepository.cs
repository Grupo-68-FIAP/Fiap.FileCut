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

			File.Delete(filePath);
			return !File.Exists(filePath);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Failed to delete file {fileName} for user {userId}.", ex);
		}
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

	public async Task<IFormFile> GetAsync(Guid userId, string fileName, CancellationToken cancellationToken)
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
		if (file == null)
			throw new ArgumentNullException(nameof(file));

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

	private void EnsureDirectoryExists(string path)
	{
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
	}
}