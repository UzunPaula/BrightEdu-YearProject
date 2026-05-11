namespace BrightEdu.Application.DTOs;

public sealed record StartQuizAttemptRequestDto(Guid QuizId);

public sealed record SubmitQuizAttemptAnswerDto(
    Guid QuestionId,
    Guid? SelectedOptionId,
    string? TextAnswer);

public sealed record SubmitQuizAttemptRequestDto(
    Guid AttemptId,
    IReadOnlyList<SubmitQuizAttemptAnswerDto> Answers);

public sealed record QuizAttemptResultDto(
    Guid AttemptId,
    decimal? Score,
    bool Passed,
    int AttemptNumber,
    DateTime StartedAt,
    DateTime? SubmittedAt);

public sealed record QuizHistoryItemDto(
    Guid AttemptId,
    Guid QuizId,
    int AttemptNumber,
    decimal? Score,
    bool Passed,
    DateTime StartedAt,
    DateTime? SubmittedAt);

public sealed record SubmitQuizAttemptResultDto(
    Guid AttemptId,
    decimal Score,
    bool Passed,
    int CorrectAnswers,
    int TotalQuestions,
    string Message);
