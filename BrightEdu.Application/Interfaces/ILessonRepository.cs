using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Interfaces;

public interface ILessonRepository
{
    Task<Lesson?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Lesson lesson, CancellationToken ct = default);
}