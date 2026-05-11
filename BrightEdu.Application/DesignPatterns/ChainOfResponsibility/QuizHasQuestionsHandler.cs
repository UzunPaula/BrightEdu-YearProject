namespace BrightEdu.Application.DesignPatterns.ChainOfResponsibility;

/// <summary>
/// Handler concret — verifică că quiz-ul conține cel puțin o întrebare.
/// </summary>
public sealed class QuizHasQuestionsHandler : QuizAttemptValidationHandler
{
    public override Task<string?> HandleAsync(QuizAttemptStartContext context, CancellationToken ct)
    {
        if (context.Quiz.Questions.Count == 0)
            return Task.FromResult<string?>("Quiz-ul nu conține întrebări.");

        return PassToNextAsync(context, ct);
    }
}
