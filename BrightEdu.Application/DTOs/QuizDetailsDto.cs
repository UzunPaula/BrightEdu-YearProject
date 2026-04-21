namespace BrightEdu.Application.DTOs;

public sealed record QuizDetailsDto(
    Guid Id,
    string Title,
    Guid LessonId);