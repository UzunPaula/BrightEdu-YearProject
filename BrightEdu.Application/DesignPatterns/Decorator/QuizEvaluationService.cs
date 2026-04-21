using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;

namespace BrightEdu.Application.DesignPatterns.Decorator;

// Serviciul real care face evaluarea pe baza datelor din baza de date.
public sealed class QuizEvaluationService : IQuizEvaluationService
{
    private readonly IQuizRepository _quizRepository;

    public QuizEvaluationService(IQuizRepository quizRepository)
    {
        _quizRepository = quizRepository;
    }

    public async Task<QuizEvaluationResultDto> EvaluateAsync(
        EvaluateQuizRequestDto request,
        CancellationToken ct = default)
    {
        // Luăm quiz-ul real din baza de date.
        var quiz = await _quizRepository.GetByIdAsync(request.QuizId, ct);

        if (quiz is null)
            throw new Exception("Quiz inexistent.");

        int score = 0;
        int total = quiz.Questions.Count;

        foreach (var question in quiz.Questions)
        {
            // Căutăm răspunsul corect pentru fiecare întrebare.
            var correctAnswer = question.Answers.First(a => a.IsCorrect);

            // Dacă studentul a selectat răspunsul corect, creștem scorul.
            if (request.SelectedAnswerIds.Contains(correctAnswer.Id))
                score++;
        }

        return new QuizEvaluationResultDto(
            score,
            total,
            $"Scor: {score}/{total}");
    }
}