namespace BrightEdu.Application.DTOs;

// Datele specifice întrebării, adică partea proprie
public sealed record RenderQuestionRequestDto(
    string QuestionText,
    string QuestionType);