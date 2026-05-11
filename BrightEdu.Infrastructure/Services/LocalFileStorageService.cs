using BrightEdu.Application.Interfaces;

namespace BrightEdu.Infrastructure.Services;

public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly string _uploadsRoot;

    public LocalFileStorageService(string uploadsRoot)
    {
        _uploadsRoot = uploadsRoot;
        Directory.CreateDirectory(_uploadsRoot);
    }

    public async Task<(string storedFileName, string relativePath)> SaveAsync(
        Stream stream, string originalFileName, string mimeType, CancellationToken ct = default)
    {
        var ext = Path.GetExtension(originalFileName);
        var storedFileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(_uploadsRoot, storedFileName);

        await using var file = File.Create(fullPath);
        await stream.CopyToAsync(file, ct);

        return (storedFileName, $"/uploads/{storedFileName}");
    }

    public Task DeleteAsync(string relativePath, CancellationToken ct = default)
    {
        var fileName = Path.GetFileName(relativePath);
        var fullPath = Path.Combine(_uploadsRoot, fileName);
        if (File.Exists(fullPath))
            File.Delete(fullPath);
        return Task.CompletedTask;
    }
}
