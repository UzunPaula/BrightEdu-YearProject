using BrightEdu.Application.DTOs;

namespace BrightEdu.Application.DesignPatterns.Decorator;

// Decoratorul de bază.
// El doar transmite apelul mai departe.
public abstract class QuizEvaluationDecorator : IQuizEvaluationService
{
    protected readonly IQuizEvaluationService Inner;

    protected QuizEvaluationDecorator(IQuizEvaluationService inner)
    {
        Inner = inner;
    }

    public virtual Task<QuizEvaluationResultDto> EvaluateAsync(
        EvaluateQuizRequestDto request,
        CancellationToken ct = default)
    {
        return Inner.EvaluateAsync(request, ct);
    }
}