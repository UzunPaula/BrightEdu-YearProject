using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Domain.Enums;
using BrightEdu.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BrightEdu.Infrastructure.Repositories;

public sealed class CourseCatalogRepository : ICourseCatalogRepository
{
    private readonly AppDbContext _dbContext;

    public CourseCatalogRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Course>> GetPublishedCoursesAsync(CancellationToken ct = default)
    {
        return await _dbContext.Courses
            .Include(x => x.Translations)
            .Include(x => x.ThumbnailMediaAsset)
            .Include(x => x.Modules)
            .ThenInclude(x => x.Translations)
            .Include(x => x.Lessons)
            .ThenInclude(x => x.Translations)
            .Include(x => x.Lessons)
            .ThenInclude(x => x.Quiz)
            .Where(x => x.State == CourseState.Published)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Course?> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        return await _dbContext.Courses
            .Include(x => x.Translations)
            .Include(x => x.Modules)
            .ThenInclude(x => x.Translations)
            .Include(x => x.Lessons)
            .ThenInclude(x => x.Translations)
            .Include(x => x.Lessons)
            .ThenInclude(x => x.Quiz)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Slug == slug.Trim().ToLower(), ct);
    }
}
