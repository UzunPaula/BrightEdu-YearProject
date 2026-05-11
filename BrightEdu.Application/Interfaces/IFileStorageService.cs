namespace BrightEdu.Application.Interfaces;

public interface IFileStorageService
{
    Task<(string storedFileName, string relativePath)> SaveAsync(Stream stream, string originalFileName, string mimeType, CancellationToken ct = default);
    Task DeleteAsync(string relativePath, CancellationToken ct = default);
}
