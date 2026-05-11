using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.DesignPatterns.Strategy;

/// <summary>
/// Strategy concretă — fiecare întrebare are aceeași pondere (scor uniform).
/// </summary>
public sealed class UniformScoringStrategy : IQuizScoringStrategy
{
    public decimal CalculateScore(
        IReadOnlyCollection<Question> questions,
        IReadOnlyCollection<QuizAttemptAnswer> answers)
    {
        if (questions.Count == 0) return 0m;

        var correct = 0;
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
                correct++;
        }

        return Math.Round((decimal)correct / questions.Count * 100m, 2);
    }
}
