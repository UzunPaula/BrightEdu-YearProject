using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Enums;

namespace BrightEdu.Application.Features.Catalog;

public interface IPublicCourseCatalogService
{
    Task<IReadOnlyList<CourseCardDto>> GetCoursesAsync(string lang = "ro", CancellationToken ct = default);
    Task<CourseDetailsDto?> GetBySlugAsync(string slug, string lang = "ro", CancellationToken ct = default);
}

public sealed class PublicCourseCatalogService : IPublicCourseCatalogService
{
    private readonly ICourseCatalogRepository _courseCatalogRepository;

    public PublicCourseCatalogService(ICourseCatalogRepository courseCatalogRepository)
    {
        _courseCatalogRepository = courseCatalogRepository;
    }

    public async Task<IReadOnlyList<CourseCardDto>> GetCoursesAsync(string lang = "ro", CancellationToken ct = default)
    {
        var courses = await _courseCatalogRepository.GetPublishedCoursesAsync(ct);
        var lc = ParseLang(lang);

        return courses
            .OrderBy(x => x.Title)
            .Select(course =>
            {
                var t = course.GetTranslation(lc) ?? course.GetTranslation(LanguageCode.Ro) ?? course.Translations.FirstOrDefault();
                return new CourseCardDto(
                    course.Id,
                    course.Slug,
                    course.Level,
                    t?.Title ?? course.Slug,
                    t?.ShortDescription,
                    course.State.ToString(),
                    course.ThumbnailMediaAsset is null ? null : $"/media/{course.ThumbnailMediaAsset.StoredFileName}");
            })
            .ToList()
            .AsReadOnly();
    }

    public async Task<CourseDetailsDto?> GetBySlugAsync(string slug, string lang = "ro", CancellationToken ct = default)
    {
        var course = await _courseCatalogRepository.GetBySlugAsync(slug, ct);
        if (course is null)
            return null;

        var lc = ParseLang(lang);

        var ct2 = course.GetTranslation(lc) ?? course.GetTranslation(LanguageCode.Ro) ?? course.Translations.FirstOrDefault();

        var moduleDtos = course.Modules
            .OrderBy(x => x.Order)
            .Select(module =>
            {
                var mt = module.GetTranslation(lc) ?? module.GetTranslation(LanguageCode.Ro) ?? module.Translations.FirstOrDefault();
                return new CourseModuleDetailsDto(
                    module.Id,
                    module.Order,
                    mt?.Title ?? module.Title,
                    mt?.Description,
                    course.Lessons
                        .Where(lesson => lesson.ModuleId == module.Id)
                        .OrderBy(lesson => lesson.Order)
                        .Select(l => MapLessonPreview(l, lc))
                        .ToList());
            })
            .ToList();

        var standaloneLessons = course.Lessons
            .Where(x => x.ModuleId is null)
            .OrderBy(x => x.Order)
            .Select(l => MapLessonPreview(l, lc))
            .ToList();

        return new CourseDetailsDto(
            course.Id,
            course.Slug,
            course.Level,
            ct2?.Title ?? course.Slug,
            ct2?.ShortDescription,
            ct2?.FullDescription,
            course.State.ToString(),
            moduleDtos,
            standaloneLessons);
    }

    private static LessonPreviewDto MapLessonPreview(Domain.Entities.Lesson lesson, LanguageCode lc)
    {
        var lt = lesson.GetTranslation(lc) ?? lesson.GetTranslation(LanguageCode.Ro) ?? lesson.Translations.FirstOrDefault();
        return new(
            lesson.Id,
            lesson.ModuleId,
            lesson.Order,
            lt?.Title ?? lesson.Title,
            lt?.Summary ?? string.Empty,
            lesson.EstimatedMinutes,
            lesson.Quiz is not null);
    }

    private static LanguageCode ParseLang(string lang) => lang.ToLowerInvariant() switch
    {
        "en" => LanguageCode.En,
        "ru" => LanguageCode.Ru,
        _ => LanguageCode.Ro
    };
}
