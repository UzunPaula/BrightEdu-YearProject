using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Interfaces;

public interface ILessonProgressRepository
{
    Task<LessonProgress?> GetAsync(Guid studentId, Guid lessonId, CancellationToken ct = default);
    Task<IReadOnlyList<LessonProgress>> GetForStudentAsync(Guid studentId, CancellationToken ct = default);
    Task AddAsync(LessonProgress progress, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
