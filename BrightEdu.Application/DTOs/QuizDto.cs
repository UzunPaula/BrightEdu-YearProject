namespace BrightEdu.Application.DTOs;

// DTO pentru afișarea unui quiz împreună cu întrebările sale.
public sealed record QuizDto(
    Guid Id,
    string Title,
    IReadOnlyList<QuestionDto> Questions);