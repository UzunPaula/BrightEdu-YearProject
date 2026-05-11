using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;

namespace BrightEdu.Application.Features.Lessons;

public interface IPublicLessonService
{
    Task<LessonDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
}

public sealed class PublicLessonService : IPublicLessonService
{
    private readonly ILessonRepository _lessonRepository;

    public PublicLessonService(ILessonRepository lessonRepository)
    {
        _lessonRepository = lessonRepository;
    }

    public async Task<LessonDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id, ct);
        if (lesson is null)
            return null;

        return new LessonDetailsDto(
            lesson.Id,
            lesson.CourseId,
            lesson.ModuleId,
            lesson.Order,
            lesson.Title,
            lesson.GetTranslation(BrightEdu.Domain.Enums.LanguageCode.Ro)?.Summary ?? string.Empty,
            lesson.EstimatedMinutes,
            lesson.CodeEditorEnabled,
            lesson.State.ToString(),
            lesson.ContentBlocks
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
                    x.MediaAssetId,
                    x.MediaAsset?.RelativePath ?? string.Empty))
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
}
