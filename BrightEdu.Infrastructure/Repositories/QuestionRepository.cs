using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BrightEdu.Infrastructure.Repositories;

public sealed class QuestionRepository : IQuestionRepository
{
    private readonly AppDbContext _db;

    public QuestionRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Question?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Questions
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == id, ct);
    }
}