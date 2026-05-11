namespace BrightEdu.Application.DTOs;

// ─── Course ──────────────────────────────────────────────────────────────────

public sealed record CreateCourseRequest(
    string Title,
    string? ShortDescription,
    string? FullDescription,
    string Level);

public sealed record UpdateCourseRequest(
    string Title,
    string? ShortDescription,
    string? FullDescription,
    string Level);

public sealed record AdminCourseDto(
    Guid Id,
    string Slug,
    string Level,
    string Title,
    string? ShortDescription,
    string? FullDescription,
    string State,
    bool IsPublished,
    int ModuleCount,
    int LessonCount,
    DateTime CreatedAt,
    DateTime UpdatedAt);

// ─── Module ───────────────────────────────────────────────────────────────────

public sealed record CreateModuleRequest(
    Guid CourseId,
    int Order,
    string Title,
    string? Description);

public sealed record UpdateModuleRequest(
    string Title,
    string? Description,
    int Order);

public sealed record AdminModuleDto(
    Guid Id,
    Guid CourseId,
    int Order,
    string Title,
    string? Description,
    string State,
    int LessonCount);

// ─── Lesson ───────────────────────────────────────────────────────────────────

public sealed record CreateLessonRequest(
    Guid CourseId,
    Guid? ModuleId,
    int Order,
    string Title,
    string Summary,
    int EstimatedMinutes);

public sealed record UpdateLessonRequest(
    string Title,
    string Summary,
    int Order,
    int EstimatedMinutes,
    bool CodeEditorEnabled);

public sealed record AdminLessonDto(
    Guid Id,
    Guid CourseId,
    Guid? ModuleId,
    int Order,
    string Title,
    string Summary,
    int EstimatedMinutes,
    bool CodeEditorEnabled,
    string State,
    bool HasQuiz);

// ─── Quiz ─────────────────────────────────────────────────────────────────────

public sealed record CreateQuizForLessonRequest(Guid LessonId, string Title, int PassingScore, int MaxAttempts, bool ShuffleQuestions, bool ShuffleAnswers);
public sealed record CreateQuizForModuleRequest(Guid ModuleId, string Title, int PassingScore, int MaxAttempts, bool ShuffleQuestions, bool ShuffleAnswers);
public sealed record UpdateQuizRequest(string Title, int PassingScore, int MaxAttempts, bool ShuffleQuestions, bool ShuffleAnswers);
public sealed record AdminQuizDto(Guid Id, Guid? LessonId, Guid? ModuleId, string Title, int PassingScore, int MaxAttempts, bool ShuffleQuestions, bool ShuffleAnswers, string State, int QuestionCount);

// ─── Question ─────────────────────────────────────────────────────────────────

public sealed record AnswerInputDto(string Text, bool IsCorrect, int Order);
public sealed record CreateQuestionRequest(Guid QuizId, string Text, string Type, int Order, int Points, IReadOnlyList<AnswerInputDto> Answers);
public sealed record UpdateQuestionRequest(string Text, string Type, int Order, int Points, IReadOnlyList<AnswerInputDto> Answers);
public sealed record AdminAnswerDto(Guid Id, string Text, bool IsCorrect, int Order);
public sealed record AdminQuestionDto(Guid Id, Guid QuizId, string Text, string Type, int Order, int Points, IReadOnlyList<AdminAnswerDto> Answers);

// ─── Lesson Content ───────────────────────────────────────────────────────────

public sealed record AdminContentBlockDto(Guid Id, string BlockType, int Order, string ConfigJson);
public sealed record AdminAttachmentDto(Guid Id, Guid MediaAssetId, string DisplayName, string Url);
public sealed record AdminLessonFullDto(
    Guid Id,
    Guid CourseId,
    Guid? ModuleId,
    int Order,
    string Title,
    string Summary,
    int EstimatedMinutes,
    bool CodeEditorEnabled,
    string State,
    bool HasQuiz,
    IReadOnlyList<AdminContentBlockDto> ContentBlocks,
    IReadOnlyList<AdminAttachmentDto> Attachments);

public sealed record ContentBlockInputDto(string BlockType, int Order, string ConfigJson);
public sealed record SetContentBlocksRequest(IReadOnlyList<ContentBlockInputDto> Blocks);
public sealed record AddAttachmentRequest(Guid MediaAssetId, string DisplayName);

// ─── Media ───────────────────────────────────────────────────────────────────

public sealed record AdminMediaAssetDto(
    Guid Id,
    string FileName,
    string Url,
    string MimeType,
    long SizeInBytes,
    DateTime CreatedAt);
