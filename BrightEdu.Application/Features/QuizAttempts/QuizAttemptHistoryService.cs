using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;

namespace BrightEdu.Application.Features.QuizAttempts;

public interface IQuizAttemptHistoryService
{
    Task<IReadOnlyList<QuizHistoryItemDto>> GetForStudentAsync(Guid quizId, Guid studentId, CancellationToken ct = default);
}

public sealed class QuizAttemptHistoryService : IQuizAttemptHistoryService
{
    private readonly IQuizAttemptRepository _quizAttemptRepository;

    public QuizAttemptHistoryService(IQuizAttemptRepository quizAttemptRepository)
    {
        _quizAttemptRepository = quizAttemptRepository;
    }

    public async Task<IReadOnlyList<QuizHistoryItemDto>> GetForStudentAsync(Guid quizId, Guid studentId, CancellationToken ct = default)
    {
        var attempts = await _quizAttemptRepository.GetForStudentByQuizAsync(quizId, studentId, ct);

        return attempts
            .Select(attempt =>
            {
                var quiz = attempt.Quiz;
                IReadOnlyList<AttemptQuestionResultDto>? questionResults = null;

                if (quiz?.ShowMistakesAfterAttempt == true && attempt.SubmittedAt is not null)
                {
                    var answerMap = attempt.Answers.ToDictionary(a => a.QuestionId);

                    var allResults = quiz.Questions
                        .OrderBy(q => q.Order)
                        .Select(q =>
                        {
                            var correctAnswer = q.Answers.FirstOrDefault(a => a.IsCorrect);
                            answerMap.TryGetValue(q.Id, out var given);
                            var selectedAnswer = given?.SelectedOptionId is not null
                                ? q.Answers.FirstOrDefault(a => a.Id == given.SelectedOptionId)
                                : null;
                            var isCorrect = given?.SelectedOptionId == correctAnswer?.Id;

                            return new AttemptQuestionResultDto(
                                q.Id,
                                q.Text,
                                given?.SelectedOptionId,
                                selectedAnswer?.Text,
                                quiz.ShowCorrectAnswer ? correctAnswer?.Id : null,
                                quiz.ShowCorrectAnswer ? correctAnswer?.Text : null,
                                isCorrect);
                        })
                        .ToList();

                    questionResults = quiz.ShowOnlyWrongAnswers
                        ? allResults.Where(r => !r.IsCorrect).ToList()
                        : allResults;
                }

                return new QuizHistoryItemDto(
                    attempt.Id,
                    attempt.QuizId,
                    attempt.AttemptNumber,
                    attempt.Score,
                    attempt.Passed,
                    attempt.StartedAt,
                    attempt.SubmittedAt,
                    questionResults);
            })
            .ToList()
            .AsReadOnly();
    }
}
