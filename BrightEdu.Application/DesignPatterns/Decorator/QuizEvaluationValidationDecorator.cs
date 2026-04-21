using BrightEdu.Application.DTOs;

namespace BrightEdu.Application.DesignPatterns.Decorator;

// Decorator pentru validarea datelor primite de la utilizator.
public sealed class QuizEvaluationValidationDecorator : QuizEvaluationDecorator
{
    public QuizEvaluationValidationDecorator(IQuizEvaluationService inner)
        : base(inner)
    {
    }

    public override async Task<QuizEvaluationResultDto> EvaluateAsync(
        EvaluateQuizRequestDto request,
        CancellationToken ct = default)
    {
        // Verificăm dacă există răspunsuri selectate.
        if (request.SelectedAnswerIds is null || request.SelectedAnswerIds.Count == 0)
            throw new ArgumentException("Nu ai selectat niciun răspuns.");

        return await base.EvaluateAsync(request, ct);
    }
}