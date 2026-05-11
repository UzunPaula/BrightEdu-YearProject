using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BrightEdu.Infrastructure.Repositories;

public sealed class AdminCourseRepository : IAdminCourseRepository
{
    private readonly AppDbContext _dbContext;

    public AdminCourseRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken ct = default)
    {
        return await _dbContext.Courses
            .Include(x => x.Translations)
            .Include(x => x.Modules)
            .ThenInclude(x => x.Lessons)
            .Include(x => x.Lessons)
            .OrderByDescending(x => x.CreatedAt)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Course?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.Courses
            .Include(x => x.Translations)
            .Include(x => x.Modules)
            .ThenInclude(x => x.Lessons)
            .Include(x => x.Lessons)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task AddAsync(Course course, CancellationToken ct = default)
    {
        await _dbContext.Courses.AddAsync(course, ct);
    }

    public async Task SaveAsync(CancellationToken ct = default)
    {
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null, CancellationToken ct = default)
    {
        return await _dbContext.Courses
            .AnyAsync(x => x.Slug == slug && (excludeId == null || x.Id != excludeId.Value), ct);
    }
}
