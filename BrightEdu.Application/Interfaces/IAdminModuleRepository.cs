using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Interfaces;

public interface IAdminModuleRepository
{
    Task<IReadOnlyList<Module>> GetByCourseIdAsync(Guid courseId, CancellationToken ct = default);
    Task<Module?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Module module, CancellationToken ct = default);
    Task RemoveAsync(Module module, CancellationToken ct = default);
    Task SaveAsync(CancellationToken ct = default);
}
