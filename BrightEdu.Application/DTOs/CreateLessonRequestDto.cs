namespace BrightEdu.Application.DTOs;

public sealed record CreateLessonRequestDto(
    string Title,
    string Content,
    int Order,
    Guid CourseId);