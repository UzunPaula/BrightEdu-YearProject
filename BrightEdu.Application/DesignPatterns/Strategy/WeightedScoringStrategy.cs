using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.DesignPatterns.Strategy;

/// <summary>
/// Strategy concretă — fiecare întrebare contribuie proporțional cu numărul de puncte (Points).
/// </summary>
public sealed class WeightedScoringStrategy : IQuizScoringStrategy
{
    public decimal CalculateScore(
        IReadOnlyCollection<Question> questions,
        IReadOnlyCollection<QuizAttemptAnswer> answers)
    {
        var totalPoints = questions.Sum(q => q.Points);
        if (totalPoints == 0) return 0m;

        var earnedPoints = 0;
        foreach (var question in questions)
        {
            var selectedIds = answers
                .Where(a => a.QuestionId == question.Id && a.SelectedOptionId.HasValue)
                .Select(a => a.SelectedOptionId!.Value)
                .ToHashSet();

            var correctIds = question.Answers
                .Where(a => a.IsCorrect)
                .Select(a => a.Id)
                .ToHashSet();

            if (selectedIds.SetEquals(correctIds))
                earnedPoints += question.Points;
        }

        return Math.Round((decimal)earnedPoints / totalPoints * 100m, 2);
    }
}
