using BrightEdu.Application.DesignPatterns.Observer;
using BrightEdu.Application.DesignPatterns.Strategy;
using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Features.QuizAttempts;

public interface ISubmitQuizAttemptService
{
    Task<SubmitQuizAttemptResultDto> SubmitAsync(Guid studentId, SubmitQuizAttemptRequestDto request, CancellationToken ct = default);
}

public sealed class SubmitQuizAttemptService : ISubmitQuizAttemptService
{
    private readonly IQuizAttemptRepository _quizAttemptRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly QuizScoringContext _scoringContext;
    private readonly IEnumerable<IQuizResultObserver> _observers;

    public SubmitQuizAttemptService(
        IQuizAttemptRepository quizAttemptRepository,
        IQuizRepository quizRepository,
        QuizScoringContext scoringContext,
        IEnumerable<IQuizResultObserver> observers)
    {
        _quizAttemptRepository = quizAttemptRepository;
        _quizRepository = quizRepository;
        _scoringContext = scoringContext;
        _observers = observers;
    }

    public async Task<SubmitQuizAttemptResultDto> SubmitAsync(Guid studentId, SubmitQuizAttemptRequestDto request, CancellationToken ct = default)
    {
        var attempt = await _quizAttemptRepository.GetByIdAsync(request.AttemptId, ct);
        if (attempt is null || attempt.StudentId != studentId)
            throw new InvalidOperationException("Încercarea nu a fost găsită.");

        var quiz = await _quizRepository.GetByIdAsync(attempt.QuizId, ct);
        if (quiz is null)
            throw new InvalidOperationException("Quiz inexistent.");

        var submittedAnswers = request.Answers
            .Select(a => new QuizAttemptAnswer(Guid.NewGuid(), attempt.Id, a.QuestionId, a.SelectedOptionId, a.TextAnswer))
            .ToList();

        // Strategy — calculul scorului delegat strategiei configurate
        var score = _scoringContext.Calculate(quiz.Questions, submittedAnswers);
        var passed = score >= quiz.PassingScore;

        foreach (var answer in submittedAnswers)
            _quizAttemptRepository.AddAnswer(answer);

        attempt.Submit(score, passed);
        await _quizAttemptRepository.SaveChangesAsync(ct);

        // Observer — notifică toți observatorii despre rezultatul quiz-ului
        foreach (var observer in _observers)
            await observer.OnQuizSubmittedAsync(studentId, quiz.LessonId, passed, ct);

        return new SubmitQuizAttemptResultDto(
            attempt.Id,
            score,
            passed,
            submittedAnswers.Count,
            quiz.Questions.Count,
            passed
                ? "Quiz trimis cu succes. Ai trecut."
                : "Quiz trimis. Nu ai atins încă pragul minim.");
    }
}
