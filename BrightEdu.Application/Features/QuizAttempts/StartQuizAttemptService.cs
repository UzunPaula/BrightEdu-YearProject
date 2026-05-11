using BrightEdu.Application.DesignPatterns.ChainOfResponsibility;
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

        // Chain of Responsibility — validare secvențială înainte de a porni quiz-ul
        var activeCheck = new QuizActiveHandler();
        var questionsCheck = new QuizHasQuestionsHandler();
        var attemptsCheck = new AttemptsRemainingHandler();
        activeCheck.SetNext(questionsCheck).SetNext(attemptsCheck);

        var context = new QuizAttemptStartContext
        {
            Quiz = quiz,
            StudentId = studentId,
            ExistingAttemptsCount = existingAttempts
        };

        var validationError = await activeCheck.HandleAsync(context, ct);
        if (validationError is not null)
            throw new InvalidOperationException(validationError);

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
