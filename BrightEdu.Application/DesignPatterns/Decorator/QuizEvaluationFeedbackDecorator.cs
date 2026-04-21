using BrightEdu.Application.DTOs;

namespace BrightEdu.Application.DesignPatterns.Decorator;

// Decorator pentru adăugarea feedback-ului final.
public sealed class QuizEvaluationFeedbackDecorator : QuizEvaluationDecorator
{
    public QuizEvaluationFeedbackDecorator(IQuizEvaluationService inner)
        : base(inner)
    {
    }

    public override async Task<QuizEvaluationResultDto> EvaluateAsync(
        EvaluateQuizRequestDto request,
        CancellationToken ct = default)
    {
        // Luăm mai întâi rezultatul calculat de serviciul precedent.
        var result = await base.EvaluateAsync(request, ct);

        string message;

        // Construim un mesaj mai util pentru utilizator.
        if (result.Score == result.Total)
            message = "Excelent, toate răspunsurile sunt corecte.";
        else if (result.Score >= result.Total / 2.0)
            message = "Rezultat bun, dar mai ai noțiuni de consolidat.";
        else
            message = "Mai repetă lecția și încearcă din nou.";

        return new QuizEvaluationResultDto(
            result.Score,
            result.Total,
            message);
    }
}