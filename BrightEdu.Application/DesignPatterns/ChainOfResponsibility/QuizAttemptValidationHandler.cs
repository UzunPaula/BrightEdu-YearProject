using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.DesignPatterns.ChainOfResponsibility;

// Chain of Responsibility — handler abstract pentru validarea lansării unui quiz.
// Fiecare handler fie rezolvă cererea, fie o transmite mai departe.
public abstract class QuizAttemptValidationHandler
{
    private QuizAttemptValidationHandler? _next;

    public QuizAttemptValidationHandler SetNext(QuizAttemptValidationHandler next)
    {
        _next = next;
        return next;
    }

    public abstract Task<string?> HandleAsync(QuizAttemptStartContext context, CancellationToken ct);

    protected async Task<string?> PassToNextAsync(QuizAttemptStartContext context, CancellationToken ct)
        => _next is null ? null : await _next.HandleAsync(context, ct);
}

/// <summary>
/// Contextul transmis prin lanț — conține datele necesare validării.
/// </summary>
public sealed class QuizAttemptStartContext
{
    public Quiz Quiz { get; init; } = null!;
    public Guid StudentId { get; init; }
    public int ExistingAttemptsCount { get; init; }
}

