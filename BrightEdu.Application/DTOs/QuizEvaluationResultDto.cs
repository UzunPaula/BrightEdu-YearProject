namespace BrightEdu.Application.DTOs;

// Rezultatul final al evaluării quiz-ului.
public sealed record QuizEvaluationResultDto(
    int Score,
    int Total,
    string Message);