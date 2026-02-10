using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Interfaces;

// ISP: interfață mică doar pentru scriere
public interface ICourseWriteRepository
{
    Task AddAsync(Course course, CancellationToken ct = default);
}