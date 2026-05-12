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
            .Include(x => x.Lesson).ThenInclude(l => l.Translations)
            .Include(x => x.Lesson).ThenInclude(l => l.Course).ThenInclude(c => c.Translations)
            .FirstOrDefaultAsync(x => x.StudentId == studentId && x.LessonId == lessonId, ct);
    }

    public async Task<IReadOnlyList<LessonProgress>> GetForStudentAsync(Guid studentId, CancellationToken ct = default)
    {
        return await _dbContext.LessonProgresses
            .Include(x => x.Lesson).ThenInclude(l => l.Translations)
            .Include(x => x.Lesson).ThenInclude(l => l.Course).ThenInclude(c => c.Translations)
            .Where(x => x.StudentId == studentId)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task AddAsync(LessonProgress progress, CancellationToken ct = default)
    {
        try
        {
            await _dbContext.LessonProgresses.AddAsync(progress, ct);
            await _dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (IsDuplicateKey(ex))
        {
            // Race condition: two concurrent requests tried to create progress for the same lesson.
            // Clear the failed entity from the tracker so subsequent GetAsync works correctly.
            _dbContext.ChangeTracker.Clear();
        }
    }

    private static bool IsDuplicateKey(DbUpdateException ex)
        => ex.InnerException?.Message.Contains("duplicate key") == true
        || ex.InnerException?.Message.Contains("IX_LessonProgresses") == true;

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _dbContext.SaveChangesAsync(ct);
}
