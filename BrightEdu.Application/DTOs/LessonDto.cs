namespace BrightEdu.Application.DTOs;

public sealed record LessonDto(Guid Id, string Title, IReadOnlyList<LessonStepDto> Steps);