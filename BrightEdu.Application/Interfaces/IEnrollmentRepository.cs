using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Interfaces;

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetAsync(Guid studentId, Guid courseId, CancellationToken ct = default);
    Task<IReadOnlyList<Enrollment>> GetForStudentAsync(Guid studentId, CancellationToken ct = default);
    Task AddAsync(Enrollment enrollment, CancellationToken ct = default);
    Task RemoveAsync(Enrollment enrollment, CancellationToken ct = default);
}
