using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BrightEdu.Infrastructure.Repositories;

// Repository real pentru Lesson, bazat pe EF Core.
// Acesta înlocuiește varianta in-memory atunci când vrem să lucrăm cu baza de date reală.
public sealed class LessonRepository : ILessonRepository
{
    private readonly AppDbContext _dbContext;

    public LessonRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Caută lecția după Id în baza de date.
    // Include și Quiz-ul asociat, dacă există.
    public async Task<Lesson?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.Lessons
            .Include(l => l.Course)
            .Include(l => l.Translations)
            .Include(l => l.ContentBlocks)
            .Include(l => l.Attachments)
                .ThenInclude(a => a.MediaAsset)
            .Include(l => l.Quiz)
            .ThenInclude(q => q!.Questions)
            .ThenInclude(q => q.Translations)
            .Include(l => l.Quiz)
            .ThenInclude(q => q!.Questions)
            .ThenInclude(q => q.Answers)
            .ThenInclude(a => a.Translations)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id, ct);
    }

    // Adaugă o lecție nouă în baza de date.
    public async Task AddAsync(Lesson lesson, CancellationToken ct = default)
    {
        await _dbContext.Lessons.AddAsync(lesson, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}
