namespace BrightEdu.Application.DTOs;

// DTO pentru afișarea unui răspuns în API.
public sealed record AnswerDto(
    Guid Id,
    string Text,
    bool IsCorrect);