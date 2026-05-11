using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Interfaces;

public interface IQuizAttemptRepository
{
    Task<QuizAttempt?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<int> CountForStudentAsync(Guid quizId, Guid studentId, CancellationToken ct = default);
    Task<IReadOnlyList<QuizAttempt>> GetForStudentByQuizAsync(Guid quizId, Guid studentId, CancellationToken ct = default);
    Task AddAsync(QuizAttempt attempt, CancellationToken ct = default);
    void AddAnswer(QuizAttemptAnswer answer);
    Task SaveChangesAsync(CancellationToken ct = default);
}
