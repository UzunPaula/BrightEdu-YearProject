using BrightEdu.Domain.Entities;
using BrightEdu.Domain.Enums;

namespace BrightEdu.Application.Interfaces;

public interface IAdminModuleRepository
{
    Task<IReadOnlyList<Module>> GetByCourseIdAsync(Guid courseId, CancellationToken ct = default);
    Task<Module?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Module module, CancellationToken ct = default);
    Task RemoveAsync(Module module, CancellationToken ct = default);
    Task SaveAsync(CancellationToken ct = default);
    Task UpsertTranslationAsync(Guid moduleId, LanguageCode lang, string title, string? description, CancellationToken ct = default);
}
