using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BrightEdu.Infrastructure.Repositories;

public sealed class AdminQuizRepository : IAdminQuizRepository
{
    private readonly AppDbContext _dbContext;

    public AdminQuizRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Quiz?> GetByLessonIdAsync(Guid lessonId, CancellationToken ct = default)
    {
        return await _dbContext.Quizzes
            .Include(q => q.Questions)
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.LessonId == lessonId, ct);
    }

    public async Task<Quiz?> GetByModuleIdAsync(Guid moduleId, CancellationToken ct = default)
    {
        return await _dbContext.Quizzes
            .Include(q => q.Questions)
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.ModuleId == moduleId, ct);
    }

    public async Task<Quiz?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.Quizzes
            .Include(q => q.Questions)
                .ThenInclude(q => q.Answers)
                    .ThenInclude(a => a.Translations)
            .Include(q => q.Questions)
                .ThenInclude(q => q.Translations)
            .FirstOrDefaultAsync(q => q.Id == id, ct);
    }

    public async Task AddAsync(Quiz quiz, CancellationToken ct = default)
    {
        await _dbContext.Quizzes.AddAsync(quiz, ct);
    }

    public async Task SaveAsync(CancellationToken ct = default)
    {
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(Quiz quiz, CancellationToken ct = default)
    {
        _dbContext.Quizzes.Remove(quiz);
        await SaveAsync(ct);
    }
}
