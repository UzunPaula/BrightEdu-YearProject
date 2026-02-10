using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Interfaces;

// ISP: interfață mică doar pentru citire, nu obligă clasele să implementeze metode inutile.
public interface ICourseReadRepository
{
    Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken ct = default);
}