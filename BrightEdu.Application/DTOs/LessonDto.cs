namespace BrightEdu.Application.DTOs;

// DTO simplu pentru afișarea unei lecții în API.
// Nu expunem direct entitatea Domain, ci doar datele de care avem nevoie în răspuns.
public sealed record LessonDto(
    Guid Id,
    string Title,
    string Content,
    int Order,
    Guid CourseId,
    QuizDto? Quiz);