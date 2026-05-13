using BrightEdu.Domain.Enums;

namespace BrightEdu.Application.DesignPatterns.ChainOfResponsibility;

// Handler concret — verifică că quiz-ul este în stare Published.
public sealed class QuizActiveHandler : QuizAttemptValidationHandler
{
    public override Task<string?> HandleAsync(QuizAttemptStartContext context, CancellationToken ct)
    {
        if (context.Quiz.State != QuizState.Published)
            return Task.FromResult<string?>("Quiz-ul nu este activ.");

        return PassToNextAsync(context, ct);
    }
}

