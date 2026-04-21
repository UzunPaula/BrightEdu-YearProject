namespace BrightEdu.Application.DTOs;

// Rezultatul final compus din datele proprii și cele partajate
public sealed record RenderedQuestionDto(
    string QuestionText,
    string QuestionType,
    string TypeDescription,
    string DisplayRule);