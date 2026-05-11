namespace BrightEdu.Application.DesignPatterns.ChainOfResponsibility;

/// <summary>
/// Handler concret — verifică că studentul nu a epuizat încercările maxime.
/// </summary>
public sealed class AttemptsRemainingHandler : QuizAttemptValidationHandler
{
    public override Task<string?> HandleAsync(QuizAttemptStartContext context, CancellationToken ct)
    {
        var hasUnlimitedAttempts = context.Quiz.MaxAttempts == 0;
        if (!hasUnlimitedAttempts && context.ExistingAttemptsCount >= context.Quiz.MaxAttempts)
            return Task.FromResult<string?>($"Ai atins numărul maxim de încercări ({context.Quiz.MaxAttempts}).");

        return PassToNextAsync(context, ct);
    }
}
