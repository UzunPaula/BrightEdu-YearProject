using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BrightEdu.Infrastructure.Repositories;

public sealed class QuizRepository : IQuizRepository
{
    private readonly AppDbContext _dbContext;

    public QuizRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Quiz quiz, CancellationToken ct = default)
    {
        await _dbContext.Quizzes.AddAsync(quiz, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<Quiz?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(qst => qst.Translations)
            .Include(q => q.Questions)
            .ThenInclude(qst => qst.Answers)
            .ThenInclude(ans => ans.Translations)
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == id, ct);
    }
}
