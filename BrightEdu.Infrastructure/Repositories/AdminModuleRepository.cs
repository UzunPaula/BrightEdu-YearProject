using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Domain.Enums;
using BrightEdu.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BrightEdu.Infrastructure.Repositories;

public sealed class AdminModuleRepository : IAdminModuleRepository
{
    private readonly AppDbContext _dbContext;

    public AdminModuleRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Module>> GetByCourseIdAsync(Guid courseId, CancellationToken ct = default)
    {
        return await _dbContext.Modules
            .Include(x => x.Translations)
            .Include(x => x.Lessons)
            .Where(x => x.CourseId == courseId)
            .OrderBy(x => x.Order)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Module?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.Modules
            .Include(x => x.Translations)
            .Include(x => x.Lessons)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task AddAsync(Module module, CancellationToken ct = default)
    {
        await _dbContext.Modules.AddAsync(module, ct);
    }

    public Task RemoveAsync(Module module, CancellationToken ct = default)
    {
        _dbContext.Modules.Remove(module);
        return Task.CompletedTask;
    }

    public async Task SaveAsync(CancellationToken ct = default)
    {
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task UpsertTranslationAsync(Guid moduleId, LanguageCode lang, string title, string? description, CancellationToken ct = default)
    {
        var existing = await _dbContext.ModuleTranslations
            .FirstOrDefaultAsync(t => t.ModuleId == moduleId && t.LanguageCode == lang, ct);

        if (existing is null)
            _dbContext.ModuleTranslations.Add(new ModuleTranslation(Guid.NewGuid(), moduleId, lang, title, description));
        else
            existing.Update(title, description);

        await _dbContext.SaveChangesAsync(ct);
    }
}
