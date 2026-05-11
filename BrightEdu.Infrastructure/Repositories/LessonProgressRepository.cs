using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BrightEdu.Infrastructure.Repositories;

public sealed class LessonProgressRepository : ILessonProgressRepository
{
    private readonly AppDbContext _dbContext;

    public LessonProgressRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LessonProgress?> GetAsync(Guid studentId, Guid lessonId, CancellationToken ct = default)
    {
        return await _dbContext.LessonProgresses
            .Include(x => x.Lesson)
            .ThenInclude(x => x.Course)
            .FirstOrDefaultAsync(x => x.StudentId == studentId && x.LessonId == lessonId, ct);
    }

    public async Task<IReadOnlyList<LessonProgress>> GetForStudentAsync(Guid studentId, CancellationToken ct = default)
    {
        return await _dbContext.LessonProgresses
            .Include(x => x.Lesson)
            .ThenInclude(x => x.Course)
            .Where(x => x.StudentId == studentId)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task AddAsync(LessonProgress progress, CancellationToken ct = default)
    {
        await _dbContext.LessonProgresses.AddAsync(progress, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _dbContext.SaveChangesAsync(ct);
}
