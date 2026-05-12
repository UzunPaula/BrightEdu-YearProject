using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BrightEdu.Infrastructure.Repositories;

public sealed class QuizAttemptRepository : IQuizAttemptRepository
{
    private readonly AppDbContext _dbContext;

    public QuizAttemptRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QuizAttempt?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.QuizAttempts
            .Include(x => x.Answers)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<int> CountForStudentAsync(Guid quizId, Guid studentId, CancellationToken ct = default)
    {
        return await _dbContext.QuizAttempts.CountAsync(
            x => x.QuizId == quizId && x.StudentId == studentId,
            ct);
    }

    public async Task<IReadOnlyList<QuizAttempt>> GetForStudentByQuizAsync(Guid quizId, Guid studentId, CancellationToken ct = default)
    {
        return await _dbContext.QuizAttempts
            .Include(x => x.Answers)
            .Include(x => x.Quiz)
                .ThenInclude(q => q.Questions)
                .ThenInclude(q => q.Translations)
            .Include(x => x.Quiz)
                .ThenInclude(q => q.Questions)
                .ThenInclude(q => q.Answers)
                .ThenInclude(a => a.Translations)
            .Where(x => x.QuizId == quizId && x.StudentId == studentId)
            .OrderByDescending(x => x.AttemptNumber)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<QuizAttempt>> GetAllSubmittedForStudentAsync(Guid studentId, CancellationToken ct = default)
    {
        return await _dbContext.QuizAttempts
            .Where(x => x.StudentId == studentId && x.SubmittedAt != null)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task AddAsync(QuizAttempt attempt, CancellationToken ct = default)
    {
        await _dbContext.QuizAttempts.AddAsync(attempt, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public void AddAnswer(QuizAttemptAnswer answer)
        => _dbContext.Set<QuizAttemptAnswer>().Add(answer);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _dbContext.SaveChangesAsync(ct);
}
