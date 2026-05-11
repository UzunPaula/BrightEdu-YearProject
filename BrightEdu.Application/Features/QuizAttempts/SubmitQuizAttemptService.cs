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

    public SubmitQuizAttemptService(
        IQuizAttemptRepository quizAttemptRepository,
        IQuizRepository quizRepository)
    {
        _quizAttemptRepository = quizAttemptRepository;
        _quizRepository = quizRepository;
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

        var totalQuestions = quiz.Questions.Count;
        var correctAnswers = 0;

        foreach (var question in quiz.Questions)
        {
            var selectedOptionIds = submittedAnswers
                .Where(x => x.QuestionId == question.Id && x.SelectedOptionId.HasValue)
                .Select(x => x.SelectedOptionId!.Value)
                .ToHashSet();

            var correctOptionIds = question.Answers
                .Where(x => x.IsCorrect)
                .Select(x => x.Id)
                .ToHashSet();

            if (selectedOptionIds.SetEquals(correctOptionIds))
                correctAnswers++;
        }

        var score = totalQuestions == 0 ? 0m : Math.Round((decimal)correctAnswers / totalQuestions * 100m, 2);
        var passed = score >= quiz.PassingScore;

        foreach (var answer in submittedAnswers)
            _quizAttemptRepository.AddAnswer(answer);

        attempt.Submit(score, passed);
        await _quizAttemptRepository.SaveChangesAsync(ct);

        return new SubmitQuizAttemptResultDto(
            attempt.Id,
            score,
            passed,
            correctAnswers,
            totalQuestions,
            passed
                ? "Quiz trimis cu succes. Ai trecut."
                : "Quiz trimis. Nu ai atins încă pragul minim.");
    }
}
