using BrightEdu.Application.DTOs;

namespace BrightEdu.Application.DesignPatterns.Decorator;

// Interfața comună pentru serviciul de evaluare.
public interface IQuizEvaluationService
{
    Task<QuizEvaluationResultDto> EvaluateAsync(
        EvaluateQuizRequestDto request,
        CancellationToken ct = default);
}
