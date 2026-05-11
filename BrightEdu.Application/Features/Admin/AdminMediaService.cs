using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Features.Admin;

public interface IAdminMediaService
{
    Task<IReadOnlyList<AdminMediaAssetDto>> GetAllAsync(CancellationToken ct = default);
    Task<AdminMediaAssetDto> UploadAsync(Stream stream, string fileName, string mimeType, long sizeInBytes, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public sealed class AdminMediaService : IAdminMediaService
{
    private readonly IMediaAssetRepository _repo;
    private readonly IFileStorageService _storage;

    public AdminMediaService(IMediaAssetRepository repo, IFileStorageService storage)
    {
        _repo = repo;
        _storage = storage;
    }

    public async Task<IReadOnlyList<AdminMediaAssetDto>> GetAllAsync(CancellationToken ct = default)
    {
        var assets = await _repo.GetAllAsync(ct);
        return assets.Select(ToDto).ToList();
    }

    public async Task<AdminMediaAssetDto> UploadAsync(
        Stream stream, string fileName, string mimeType, long sizeInBytes, CancellationToken ct = default)
    {
        var (storedFileName, relativePath) = await _storage.SaveAsync(stream, fileName, mimeType, ct);
        var asset = new MediaAsset(Guid.NewGuid(), fileName, storedFileName, relativePath, mimeType, sizeInBytes);
        await _repo.AddAsync(asset, ct);
        return ToDto(asset);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var asset = await _repo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Fișierul nu a fost găsit.");
        await _storage.DeleteAsync(asset.RelativePath, ct);
        await _repo.DeleteAsync(asset, ct);
    }

    private static AdminMediaAssetDto ToDto(MediaAsset a) =>
        new(a.Id, a.FileName, a.RelativePath, a.MimeType, a.SizeInBytes, a.CreatedAt);
}
