using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;

namespace BrightEdu.Application.Features.QuizAttempts;

public interface IStartQuizAttemptService
{
    Task<QuizAttemptResultDto> StartAsync(Guid quizId, Guid studentId, CancellationToken ct = default);
}

public sealed class StartQuizAttemptService : IStartQuizAttemptService
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizAttemptRepository _quizAttemptRepository;

    public StartQuizAttemptService(
        IQuizRepository quizRepository,
        IQuizAttemptRepository quizAttemptRepository)
    {
        _quizRepository = quizRepository;
        _quizAttemptRepository = quizAttemptRepository;
    }

    public async Task<QuizAttemptResultDto> StartAsync(Guid quizId, Guid studentId, CancellationToken ct = default)
    {
        var quiz = await _quizRepository.GetByIdAsync(quizId, ct);
        if (quiz is null)
            throw new InvalidOperationException("Quiz inexistent.");

        var existingAttempts = await _quizAttemptRepository.CountForStudentAsync(quizId, studentId, ct);
        var hasUnlimitedAttempts = quiz.MaxAttempts == 0;

        if (!hasUnlimitedAttempts && existingAttempts >= quiz.MaxAttempts)
            throw new InvalidOperationException("Ai atins numarul maxim de incercari permis.");

        var attempt = new Domain.Entities.QuizAttempt(Guid.NewGuid(), quizId, studentId, existingAttempts + 1);
        await _quizAttemptRepository.AddAsync(attempt, ct);

        return new QuizAttemptResultDto(
            attempt.Id,
            attempt.Score,
            attempt.Passed,
            attempt.AttemptNumber,
            attempt.StartedAt,
            attempt.SubmittedAt);
    }
}
