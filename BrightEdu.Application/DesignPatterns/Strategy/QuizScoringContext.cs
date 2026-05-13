using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.DesignPatterns.Strategy;

// Context — selectează și aplică strategia de calcul a scorului.
public sealed class QuizScoringContext
{
    private IQuizScoringStrategy _strategy;

    public QuizScoringContext(IQuizScoringStrategy strategy)
    {
        _strategy = strategy;
    }

    public void SetStrategy(IQuizScoringStrategy strategy)
    {
        _strategy = strategy;
    }

    public decimal Calculate(
        IReadOnlyCollection<Question> questions,
        IReadOnlyCollection<QuizAttemptAnswer> answers)
        => _strategy.CalculateScore(questions, answers);
}
