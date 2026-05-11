using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;

namespace BrightEdu.Application.Features.QuizAttempts;

public interface IQuizAttemptHistoryService
{
    Task<IReadOnlyList<QuizHistoryItemDto>> GetForStudentAsync(Guid quizId, Guid studentId, CancellationToken ct = default);
}

public sealed class QuizAttemptHistoryService : IQuizAttemptHistoryService
{
    private readonly IQuizAttemptRepository _quizAttemptRepository;

    public QuizAttemptHistoryService(IQuizAttemptRepository quizAttemptRepository)
    {
        _quizAttemptRepository = quizAttemptRepository;
    }

    public async Task<IReadOnlyList<QuizHistoryItemDto>> GetForStudentAsync(Guid quizId, Guid studentId, CancellationToken ct = default)
    {
        var attempts = await _quizAttemptRepository.GetForStudentByQuizAsync(quizId, studentId, ct);

        return attempts
            .Select(x => new QuizHistoryItemDto(
                x.Id,
                x.QuizId,
                x.AttemptNumber,
                x.Score,
                x.Passed,
                x.StartedAt,
                x.SubmittedAt))
            .ToList()
            .AsReadOnly();
    }
}
