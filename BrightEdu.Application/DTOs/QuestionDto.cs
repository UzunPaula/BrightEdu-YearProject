namespace BrightEdu.Application.DTOs;

// DTO pentru afișarea unei întrebări împreună cu răspunsurile sale.
public sealed record QuestionDto(
    Guid Id,
    string Text,
    IReadOnlyList<AnswerDto> Answers);