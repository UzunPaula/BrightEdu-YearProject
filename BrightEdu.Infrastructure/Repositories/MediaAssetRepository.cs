using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BrightEdu.Infrastructure.Repositories;

public sealed class MediaAssetRepository : IMediaAssetRepository
{
    private readonly AppDbContext _dbContext;

    public MediaAssetRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<MediaAsset>> GetAllAsync(CancellationToken ct = default)
        => await _dbContext.MediaAssets
            .OrderByDescending(x => x.CreatedAt)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<MediaAsset?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _dbContext.MediaAssets.FindAsync([id], ct);

    public async Task AddAsync(MediaAsset asset, CancellationToken ct = default)
    {
        await _dbContext.MediaAssets.AddAsync(asset, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(MediaAsset asset, CancellationToken ct = default)
    {
        _dbContext.MediaAssets.Remove(asset);
        await _dbContext.SaveChangesAsync(ct);
    }
}
