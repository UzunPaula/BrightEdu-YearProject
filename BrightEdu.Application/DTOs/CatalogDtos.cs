using BrightEdu.Domain.Enums;

namespace BrightEdu.Application.DTOs;

public sealed record LocalizedTextDto(
    LanguageCode Language,
    string Value);

public sealed record CourseCardDto(
    Guid Id,
    string Slug,
    string Level,
    string Title,
    string? ShortDescription,
    string State,
    string? ThumbnailUrl);

public sealed record ModuleDto(
    Guid Id,
    int Order,
    string Title,
    string? Description);

public sealed record LessonPreviewDto(
    Guid Id,
    Guid? ModuleId,
    int Order,
    string Title,
    string Summary,
    int EstimatedMinutes,
    bool HasQuiz);

public sealed record CourseModuleDetailsDto(
    Guid Id,
    int Order,
    string Title,
    string? Description,
    IReadOnlyList<LessonPreviewDto> Lessons);

public sealed record LessonContentBlockDto(
    Guid Id,
    int Order,
    string BlockType,
    string ConfigJson);

public sealed record LessonAttachmentDto(
    Guid Id,
    string DisplayName,
    Guid MediaAssetId,
    string Url);

public sealed record QuizAnswerOptionDto(
    Guid Id,
    string Text);

public sealed record QuizQuestionDto(
    Guid Id,
    string Text,
    string QuestionType,
    int Order,
    IReadOnlyList<QuizAnswerOptionDto> Options);

public sealed record LessonQuizDto(
    Guid Id,
    string Title,
    int PassingScore,
    int MaxAttempts,
    IReadOnlyList<QuizQuestionDto> Questions);

public sealed record LessonDetailsDto(
    Guid Id,
    Guid CourseId,
    Guid? ModuleId,
    int Order,
    string Title,
    string Summary,
    int EstimatedMinutes,
    bool CodeEditorEnabled,
    string State,
    IReadOnlyList<LessonContentBlockDto> ContentBlocks,
    IReadOnlyList<LessonAttachmentDto> Attachments,
    Guid? QuizId,
    LessonQuizDto? Quiz);

public sealed record CourseDetailsDto(
    Guid Id,
    string Slug,
    string Level,
    string Title,
    string? ShortDescription,
    string? FullDescription,
    string State,
    IReadOnlyList<CourseModuleDetailsDto> Modules,
    IReadOnlyList<LessonPreviewDto> StandaloneLessons);
