using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Interfaces;

public interface IMediaAssetRepository
{
    Task<IReadOnlyList<MediaAsset>> GetAllAsync(CancellationToken ct = default);
    Task<MediaAsset?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(MediaAsset asset, CancellationToken ct = default);
    Task DeleteAsync(MediaAsset asset, CancellationToken ct = default);
}
