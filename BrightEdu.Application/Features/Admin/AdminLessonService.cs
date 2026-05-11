using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Domain.Enums;

namespace BrightEdu.Application.Features.Admin;

public interface IAdminLessonService
{
    Task<IReadOnlyList<AdminLessonDto>> GetByCourseIdAsync(Guid courseId, CancellationToken ct = default);
    Task<AdminLessonDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<AdminLessonFullDto?> GetFullAsync(Guid id, CancellationToken ct = default);
    Task<AdminLessonDto> CreateAsync(CreateLessonRequest request, CancellationToken ct = default);
    Task<AdminLessonDto?> UpdateAsync(Guid id, UpdateLessonRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<bool> PublishAsync(Guid id, CancellationToken ct = default);
    Task<AdminLessonFullDto?> SetContentBlocksAsync(Guid id, string lang, SetContentBlocksRequest request, CancellationToken ct = default);
    Task<AdminAttachmentDto?> AddAttachmentAsync(Guid id, AddAttachmentRequest request, CancellationToken ct = default);
    Task<AdminAttachmentDto?> AddAttachmentLinkAsync(Guid id, AddAttachmentLinkRequest request, CancellationToken ct = default);
    Task<bool> RemoveAttachmentAsync(Guid id, Guid attachmentId, CancellationToken ct = default);
    Task<IReadOnlyList<EntityTranslationDto>> GetTranslationsAsync(Guid id, CancellationToken ct = default);
    Task<bool> UpsertTranslationAsync(Guid id, string lang, UpsertLessonTranslationRequest request, CancellationToken ct = default);
}

public sealed class AdminLessonService : IAdminLessonService
{
    private readonly IAdminLessonRepository _repository;

    public AdminLessonService(IAdminLessonRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<AdminLessonDto>> GetByCourseIdAsync(Guid courseId, CancellationToken ct = default)
    {
        var lessons = await _repository.GetByCourseIdAsync(courseId, ct);
        return lessons.OrderBy(x => x.Order).Select(Map).ToList().AsReadOnly();
    }

    public async Task<AdminLessonDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var lesson = await _repository.GetByIdAsync(id, ct);
        return lesson is null ? null : Map(lesson);
    }

    public async Task<AdminLessonDto> CreateAsync(CreateLessonRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Titlul lecției este obligatoriu.");

        if (request.Order <= 0)
            throw new ArgumentException("Ordinea trebuie să fie mai mare ca 0.");

        var lesson = new Lesson(
            Guid.NewGuid(),
            request.Title,
            request.Summary,
            request.Order,
            request.CourseId);

        if (request.ModuleId.HasValue)
            lesson.AssignModule(request.ModuleId.Value);

        if (request.EstimatedMinutes > 0)
            lesson.SetEstimatedMinutes(request.EstimatedMinutes);

        await _repository.AddAsync(lesson, ct);
        await _repository.SaveAsync(ct);

        return Map(lesson);
    }

    public async Task<AdminLessonDto?> UpdateAsync(Guid id, UpdateLessonRequest request, CancellationToken ct = default)
    {
        var lesson = await _repository.GetByIdAsync(id, ct);
        if (lesson is null) return null;

        lesson.AddTranslation(LanguageCode.Ro, request.Title, request.Summary);
        lesson.SetOrder(request.Order);

        if (request.EstimatedMinutes > 0)
            lesson.SetEstimatedMinutes(request.EstimatedMinutes);

        lesson.EnableCodeEditor(request.CodeEditorEnabled);

        await _repository.SaveAsync(ct);
        return Map(lesson);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var lesson = await _repository.GetByIdAsync(id, ct);
        if (lesson is null) return false;

        await _repository.RemoveAsync(lesson, ct);
        await _repository.SaveAsync(ct);
        return true;
    }

    public async Task<bool> PublishAsync(Guid id, CancellationToken ct = default)
    {
        var lesson = await _repository.GetByIdAsync(id, ct);
        if (lesson is null) return false;

        lesson.SetState(LessonState.Published);
        await _repository.SaveAsync(ct);
        return true;
    }

    public async Task<AdminLessonFullDto?> GetFullAsync(Guid id, CancellationToken ct = default)
    {
        var lesson = await _repository.GetFullAsync(id, ct);
        return lesson is null ? null : MapFull(lesson);
    }

    public async Task<AdminLessonFullDto?> SetContentBlocksAsync(Guid id, string lang, SetContentBlocksRequest request, CancellationToken ct = default)
    {
        var lesson = await _repository.GetFullAsync(id, ct);
        if (lesson is null) return null;

        var normalizedLang = lang.ToLowerInvariant();
        await _repository.DeleteContentBlocksAsync(id, normalizedLang, ct);

        foreach (var b in request.Blocks)
        {
            if (!Enum.TryParse<ContentBlockType>(b.BlockType, ignoreCase: true, out var blockType))
                throw new ArgumentException($"Tip bloc necunoscut: {b.BlockType}");

            _repository.AddContentBlock(new LessonContentBlock(Guid.NewGuid(), id, b.Order, blockType, b.ConfigJson, normalizedLang));
        }

        await _repository.SaveAsync(ct);
        return await GetFullAsync(id, ct);
    }

    public async Task<AdminAttachmentDto?> AddAttachmentAsync(Guid id, AddAttachmentRequest request, CancellationToken ct = default)
    {
        var lesson = await _repository.GetByIdAsync(id, ct);
        if (lesson is null) return null;

        var attachment = new LessonAttachment(Guid.NewGuid(), id, request.MediaAssetId, request.DisplayName);
        _repository.AddAttachment(attachment);
        await _repository.SaveAsync(ct);

        return new AdminAttachmentDto(attachment.Id, attachment.MediaAssetId, attachment.DisplayName, attachment.MediaAsset?.RelativePath ?? string.Empty);
    }

    public async Task<AdminAttachmentDto?> AddAttachmentLinkAsync(Guid id, AddAttachmentLinkRequest request, CancellationToken ct = default)
    {
        var lesson = await _repository.GetByIdAsync(id, ct);
        if (lesson is null) return null;

        var attachment = new LessonAttachment(Guid.NewGuid(), id, request.ExternalUrl, request.DisplayName);
        _repository.AddAttachment(attachment);
        await _repository.SaveAsync(ct);

        return new AdminAttachmentDto(attachment.Id, null, attachment.DisplayName, request.ExternalUrl);
    }

    public async Task<bool> RemoveAttachmentAsync(Guid id, Guid attachmentId, CancellationToken ct = default)
    {
        var lesson = await _repository.GetByIdAsync(id, ct);
        if (lesson is null) return false;

        await _repository.DeleteAttachmentAsync(attachmentId, ct);
        await _repository.SaveAsync(ct);
        return true;
    }

    public async Task<IReadOnlyList<EntityTranslationDto>> GetTranslationsAsync(Guid id, CancellationToken ct = default)
    {
        var lesson = await _repository.GetByIdAsync(id, ct);
        if (lesson is null) return Array.Empty<EntityTranslationDto>();

        return lesson.Translations
            .Select(t => new EntityTranslationDto(t.LanguageCode.ToString().ToLower(), t.Title, t.Summary, null))
            .ToList().AsReadOnly();
    }

    public async Task<bool> UpsertTranslationAsync(Guid id, string lang, UpsertLessonTranslationRequest request, CancellationToken ct = default)
    {
        var exists = await _repository.GetByIdAsync(id, ct);
        if (exists is null) return false;

        await _repository.UpsertTranslationAsync(id, ParseLanguage(lang), request.Title, request.Summary, ct);
        return true;
    }

    private static LanguageCode ParseLanguage(string lang) => lang.ToLowerInvariant() switch
    {
        "ro" => LanguageCode.Ro,
        "en" => LanguageCode.En,
        "ru" => LanguageCode.Ru,
        _ => throw new ArgumentException($"Limbă necunoscută: {lang}")
    };

    private static AdminLessonDto Map(Lesson lesson)
    {
        var translation = lesson.GetTranslation(LanguageCode.Ro) ?? lesson.Translations.FirstOrDefault();
        return new AdminLessonDto(
            lesson.Id,
            lesson.CourseId,
            lesson.ModuleId,
            lesson.Order,
            translation?.Title ?? string.Empty,
            translation?.Summary ?? string.Empty,
            lesson.EstimatedMinutes,
            lesson.CodeEditorEnabled,
            lesson.State.ToString(),
            lesson.Quiz is not null);
    }

    private static AdminLessonFullDto MapFull(Lesson lesson)
    {
        var translation = lesson.GetTranslation(LanguageCode.Ro) ?? lesson.Translations.FirstOrDefault();
        var blocks = lesson.ContentBlocks
            .OrderBy(b => b.Order)
            .Select(b => new AdminContentBlockDto(b.Id, b.BlockType.ToString(), b.Order, b.ConfigJson, b.Lang))
            .ToList()
            .AsReadOnly();
        var attachments = lesson.Attachments
            .Select(a => new AdminAttachmentDto(a.Id, a.MediaAssetId, a.DisplayName, a.ExternalUrl ?? a.MediaAsset?.RelativePath ?? string.Empty))
            .ToList()
            .AsReadOnly();
        return new AdminLessonFullDto(
            lesson.Id,
            lesson.CourseId,
            lesson.ModuleId,
            lesson.Order,
            translation?.Title ?? string.Empty,
            translation?.Summary ?? string.Empty,
            lesson.EstimatedMinutes,
            lesson.CodeEditorEnabled,
            lesson.State.ToString(),
            lesson.Quiz is not null,
            blocks,
            attachments);
    }
}
