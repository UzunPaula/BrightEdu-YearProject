using BrightEdu.Domain.Entities;
using BrightEdu.Domain.Enums;

namespace BrightEdu.Application.Interfaces;

public interface IAdminCourseRepository
{
    Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken ct = default);
    Task<Course?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Course course, CancellationToken ct = default);
    Task SaveAsync(CancellationToken ct = default);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null, CancellationToken ct = default);
    Task UpsertTranslationAsync(Guid courseId, LanguageCode lang, string title, string? shortDescription, string? fullDescription, CancellationToken ct = default);
}
