namespace BrightEdu.Application.DTOs;

// Cererea trimisă de student pentru evaluarea quiz-ului.
public sealed record EvaluateQuizRequestDto(
    Guid QuizId,
    List<Guid> SelectedAnswerIds);