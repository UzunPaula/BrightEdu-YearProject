using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Enums;

namespace BrightEdu.Application.Features.Lessons;

public interface IPublicLessonService
{
    Task<LessonDetailsDto?> GetByIdAsync(Guid id, string lang = "ro", CancellationToken ct = default);
}

public sealed class PublicLessonService : IPublicLessonService
{
    private readonly ILessonRepository _lessonRepository;

    public PublicLessonService(ILessonRepository lessonRepository)
    {
        _lessonRepository = lessonRepository;
    }

    public async Task<LessonDetailsDto?> GetByIdAsync(Guid id, string lang = "ro", CancellationToken ct = default)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id, ct);
        if (lesson is null)
            return null;

        var lc = ParseLang(lang);
        var lt = lesson.GetTranslation(lc) ?? lesson.GetTranslation(LanguageCode.Ro) ?? lesson.Translations.FirstOrDefault();

        return new LessonDetailsDto(
            lesson.Id,
            lesson.CourseId,
            lesson.Course?.Slug,
            lesson.ModuleId,
            lesson.Order,
            lt?.Title ?? lesson.Title,
            lt?.Summary ?? string.Empty,
            lesson.EstimatedMinutes,
            lesson.CodeEditorEnabled,
            lesson.State.ToString(),
            (lesson.ContentBlocks.Any(x => x.Lang == lang)
                ? lesson.ContentBlocks.Where(x => x.Lang == lang)
                : lesson.ContentBlocks.Where(x => x.Lang == "ro"))
                .OrderBy(x => x.Order)
                .Select(x => new LessonContentBlockDto(
                    x.Id,
                    x.Order,
                    x.BlockType.ToString(),
                    x.ConfigJson))
                .ToList(),
            lesson.Attachments
                .Select(x => new LessonAttachmentDto(
                    x.Id,
                    x.DisplayName,
                    x.MediaAssetId ?? Guid.Empty,
                    x.ExternalUrl ?? x.MediaAsset?.RelativePath ?? string.Empty))
                .ToList(),
            lesson.Quiz?.Id,
            lesson.Quiz is null
                ? null
                : new LessonQuizDto(
                    lesson.Quiz.Id,
                    lesson.Quiz.Title,
                    lesson.Quiz.PassingScore,
                    lesson.Quiz.MaxAttempts,
                    lesson.Quiz.Questions
                        .OrderBy(x => x.Order)
                        .Select(question => new QuizQuestionDto(
                            question.Id,
                            question.Text,
                            question.Type.ToString(),
                            question.Order,
                            question.Answers
                                .OrderBy(x => x.Order)
                                .Select(answer => new QuizAnswerOptionDto(
                                    answer.Id,
                                    answer.Text))
                                .ToList()))
                        .ToList()));
    }

    private static LanguageCode ParseLang(string lang) => lang.ToLowerInvariant() switch
    {
        "en" => LanguageCode.En,
        "ru" => LanguageCode.Ru,
        _ => LanguageCode.Ro
    };
}
