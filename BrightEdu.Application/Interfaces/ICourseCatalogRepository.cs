using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Interfaces;

public interface ICourseCatalogRepository
{
    Task<IReadOnlyList<Course>> GetPublishedCoursesAsync(CancellationToken ct = default);
    Task<Course?> GetBySlugAsync(string slug, CancellationToken ct = default);
}
