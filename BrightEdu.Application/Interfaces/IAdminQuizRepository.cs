using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Interfaces;

public interface IAdminQuizRepository
{
    Task<Quiz?> GetByLessonIdAsync(Guid lessonId, CancellationToken ct = default);
    Task<Quiz?> GetByModuleIdAsync(Guid moduleId, CancellationToken ct = default);
    Task<Quiz?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Quiz quiz, CancellationToken ct = default);
    Task SaveAsync(CancellationToken ct = default);
    Task RemoveAsync(Quiz quiz, CancellationToken ct = default);
}
