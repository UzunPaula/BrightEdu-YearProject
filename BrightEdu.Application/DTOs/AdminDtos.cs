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

public sealed record CreateQuizForLessonRequest(Guid LessonId, string Title, int PassingScore, int MaxAttempts, bool ShuffleQuestions, bool ShuffleAnswers, bool ShowMistakesAfterAttempt = false, bool ShowOnlyWrongAnswers = false, bool ShowCorrectAnswer = false);
public sealed record CreateQuizForModuleRequest(Guid ModuleId, string Title, int PassingScore, int MaxAttempts, bool ShuffleQuestions, bool ShuffleAnswers, bool ShowMistakesAfterAttempt = false, bool ShowOnlyWrongAnswers = false, bool ShowCorrectAnswer = false);
public sealed record UpdateQuizRequest(string Title, int PassingScore, int MaxAttempts, bool ShuffleQuestions, bool ShuffleAnswers, bool ShowMistakesAfterAttempt = false, bool ShowOnlyWrongAnswers = false, bool ShowCorrectAnswer = false);
public sealed record AdminQuizDto(Guid Id, Guid? LessonId, Guid? ModuleId, string Title, int PassingScore, int MaxAttempts, bool ShuffleQuestions, bool ShuffleAnswers, bool ShowMistakesAfterAttempt, bool ShowOnlyWrongAnswers, bool ShowCorrectAnswer, string State, int QuestionCount);

// ─── Question ─────────────────────────────────────────────────────────────────

public sealed record AnswerInputDto(string Text, bool IsCorrect, int Order);
public sealed record CreateQuestionRequest(Guid QuizId, string Text, string Type, int Order, int Points, IReadOnlyList<AnswerInputDto> Answers);
public sealed record UpdateQuestionRequest(string Text, string Type, int Order, int Points, IReadOnlyList<AnswerInputDto> Answers);
public sealed record AdminAnswerDto(Guid Id, string Text, bool IsCorrect, int Order);
public sealed record AdminQuestionDto(Guid Id, Guid QuizId, string Text, string Type, int Order, int Points, IReadOnlyList<AdminAnswerDto> Answers);

// ─── Lesson Content ───────────────────────────────────────────────────────────

public sealed record AdminContentBlockDto(Guid Id, string BlockType, int Order, string ConfigJson, string Lang);
public sealed record AdminAttachmentDto(Guid Id, Guid? MediaAssetId, string DisplayName, string Url);
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
public sealed record AddAttachmentLinkRequest(string ExternalUrl, string DisplayName);

// ─── Translations ─────────────────────────────────────────────────────────────

public sealed record EntityTranslationDto(string Lang, string Title, string? Field2, string? Field3);
public sealed record UpsertCourseTranslationRequest(string Title, string? ShortDescription, string? FullDescription);
public sealed record UpsertModuleTranslationRequest(string Title, string? Description);
public sealed record UpsertLessonTranslationRequest(string Title, string Summary);

// ─── Course Import ────────────────────────────────────────────────────────────

public sealed record CourseImportTranslationDto(string Lang, string Title, string? ShortDescription, string? FullDescription);
public sealed record ModuleImportTranslationDto(string Lang, string Title, string? Description);
public sealed record LessonImportTranslationDto(string Lang, string Title, string Summary);

public sealed record AnswerImportDto(int Order, string Text, bool IsCorrect);
public sealed record QuestionImportDto(int Order, string Text, string Type, int Points, IReadOnlyList<AnswerImportDto> Answers);
public sealed record QuizImportDto(string Title, int PassingScore, int MaxAttempts, bool ShuffleQuestions, bool ShuffleAnswers, IReadOnlyList<QuestionImportDto> Questions, bool Published = false);
public sealed record ContentBlockImportDto(string Lang, int Order, string BlockType, string ConfigJson);

public sealed record LessonImportDto(
    int Order,
    string Title,
    string Summary,
    int EstimatedMinutes,
    bool CodeEditorEnabled,
    IReadOnlyList<LessonImportTranslationDto> Translations,
    IReadOnlyList<ContentBlockImportDto> ContentBlocks,
    QuizImportDto? Quiz,
    bool Published = false);

public sealed record ModuleImportDto(
    int Order,
    string Title,
    string? Description,
    IReadOnlyList<ModuleImportTranslationDto> Translations,
    IReadOnlyList<LessonImportDto> Lessons,
    bool Published = false);

public sealed record CourseImportDto(
    string Title,
    string? ShortDescription,
    string? FullDescription,
    string Level,
    IReadOnlyList<CourseImportTranslationDto> Translations,
    IReadOnlyList<ModuleImportDto> Modules,
    IReadOnlyList<LessonImportDto> StandaloneLessons,
    bool Published = false);

public sealed record CourseImportResultDto(Guid CourseId, string Title, int ModulesImported, int LessonsImported, int QuizzesImported);

// ─── Media ───────────────────────────────────────────────────────────────────

public sealed record AdminMediaAssetDto(
    Guid Id,
    string FileName,
    string Url,
    string MimeType,
    long SizeInBytes,
    DateTime CreatedAt);
