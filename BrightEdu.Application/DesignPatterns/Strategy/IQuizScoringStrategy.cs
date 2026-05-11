using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.DesignPatterns.Strategy;

/// <summary>
/// Strategy — definește algoritmul de calcul al scorului pentru un quiz.
/// </summary>
public interface IQuizScoringStrategy
{
    decimal CalculateScore(
        IReadOnlyCollection<Question> questions,
        IReadOnlyCollection<QuizAttemptAnswer> answers);
}
