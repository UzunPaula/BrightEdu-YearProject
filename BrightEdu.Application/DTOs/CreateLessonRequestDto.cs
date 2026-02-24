namespace BrightEdu.Application.DTOs;

public sealed record CreateLessonRequestDto(
    Guid Id,
    string Title,
    IReadOnlyList<CreateLessonStepRequestDto> Steps
);

public sealed record CreateLessonStepRequestDto(
    string Type,
    Guid Id,
    int Order,
    string? Content,
    string? QuestionText,
    IReadOnlyList<string>? Options,
    int? CorrectOptionIndex
);