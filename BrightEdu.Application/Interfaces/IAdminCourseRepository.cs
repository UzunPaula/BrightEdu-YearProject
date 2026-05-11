using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Interfaces;

public interface IAdminCourseRepository
{
    Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken ct = default);
    Task<Course?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Course course, CancellationToken ct = default);
    Task SaveAsync(CancellationToken ct = default);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null, CancellationToken ct = default);
}
