using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Interfaces;

public interface IAdminQuestionRepository
{
    Task<IReadOnlyList<Question>> GetByQuizIdAsync(Guid quizId, CancellationToken ct = default);
    Task<Question?> GetByIdWithAnswersAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Question question, CancellationToken ct = default);
    Task RemoveAsync(Question question, CancellationToken ct = default);
    Task SaveAsync(CancellationToken ct = default);
    Task ReplaceAnswersAsync(Guid questionId, IReadOnlyList<Answer> newAnswers, CancellationToken ct = default);
}
