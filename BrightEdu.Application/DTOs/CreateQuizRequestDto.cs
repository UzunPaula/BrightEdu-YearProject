namespace BrightEdu.Application.DTOs;

public sealed record CreateQuizRequestDto(
    string Title,
    Guid LessonId);