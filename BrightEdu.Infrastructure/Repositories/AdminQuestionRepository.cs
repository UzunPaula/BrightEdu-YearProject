using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BrightEdu.Infrastructure.Repositories;

public sealed class AdminQuestionRepository : IAdminQuestionRepository
{
    private readonly AppDbContext _dbContext;

    public AdminQuestionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Question>> GetByQuizIdAsync(Guid quizId, CancellationToken ct = default)
    {
        return await _dbContext.Questions
            .Include(q => q.Answers)
                .ThenInclude(a => a.Translations)
            .Include(q => q.Translations)
            .Where(q => q.QuizId == quizId)
            .OrderBy(q => q.Order)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Question?> GetByIdWithAnswersAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.Questions
            .Include(q => q.Answers)
                .ThenInclude(a => a.Translations)
            .Include(q => q.Translations)
            .FirstOrDefaultAsync(q => q.Id == id, ct);
    }

    public async Task AddAsync(Question question, CancellationToken ct = default)
    {
        await _dbContext.Questions.AddAsync(question, ct);
    }

    public Task RemoveAsync(Question question, CancellationToken ct = default)
    {
        _dbContext.Questions.Remove(question);
        return Task.CompletedTask;
    }

    public async Task SaveAsync(CancellationToken ct = default)
    {
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task ReplaceAnswersAsync(Guid questionId, IReadOnlyList<Answer> newAnswers, CancellationToken ct = default)
    {
        var old = await _dbContext.Answers.Where(a => a.QuestionId == questionId).ToListAsync(ct);
        _dbContext.Answers.RemoveRange(old);
        await _dbContext.Answers.AddRangeAsync(newAnswers, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}
