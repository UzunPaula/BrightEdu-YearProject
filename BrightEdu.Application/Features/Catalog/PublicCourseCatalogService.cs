using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;

namespace BrightEdu.Application.Features.Catalog;

public interface IPublicCourseCatalogService
{
    Task<IReadOnlyList<CourseCardDto>> GetCoursesAsync(CancellationToken ct = default);
    Task<CourseDetailsDto?> GetBySlugAsync(string slug, CancellationToken ct = default);
}

public sealed class PublicCourseCatalogService : IPublicCourseCatalogService
{
    private readonly ICourseCatalogRepository _courseCatalogRepository;

    public PublicCourseCatalogService(ICourseCatalogRepository courseCatalogRepository)
    {
        _courseCatalogRepository = courseCatalogRepository;
    }

    public async Task<IReadOnlyList<CourseCardDto>> GetCoursesAsync(CancellationToken ct = default)
    {
        var courses = await _courseCatalogRepository.GetPublishedCoursesAsync(ct);

        return courses
            .OrderBy(x => x.Title)
            .Select(course => new CourseCardDto(
                course.Id,
                course.Slug,
                course.Level,
                course.Title,
                course.Description,
                course.State.ToString(),
                course.ThumbnailMediaAsset is null ? null : $"/media/{course.ThumbnailMediaAsset.StoredFileName}"))
            .ToList()
            .AsReadOnly();
    }

    public async Task<CourseDetailsDto?> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        var course = await _courseCatalogRepository.GetBySlugAsync(slug, ct);
        if (course is null)
            return null;

        var moduleDtos = course.Modules
            .OrderBy(x => x.Order)
            .Select(module => new CourseModuleDetailsDto(
                module.Id,
                module.Order,
                module.Title,
                module.GetTranslation(BrightEdu.Domain.Enums.LanguageCode.Ro)?.Description,
                course.Lessons
                    .Where(lesson => lesson.ModuleId == module.Id)
                    .OrderBy(lesson => lesson.Order)
                    .Select(MapLessonPreview)
                    .ToList()))
            .ToList();

        var standaloneLessons = course.Lessons
            .Where(x => x.ModuleId is null)
            .OrderBy(x => x.Order)
            .Select(MapLessonPreview)
            .ToList();

        return new CourseDetailsDto(
            course.Id,
            course.Slug,
            course.Level,
            course.Title,
            course.Description,
            course.GetTranslation(BrightEdu.Domain.Enums.LanguageCode.Ro)?.FullDescription,
            course.State.ToString(),
            moduleDtos,
            standaloneLessons);
    }

    private static LessonPreviewDto MapLessonPreview(Domain.Entities.Lesson lesson)
        => new(
            lesson.Id,
            lesson.ModuleId,
            lesson.Order,
            lesson.Title,
            lesson.GetTranslation(BrightEdu.Domain.Enums.LanguageCode.Ro)?.Summary ?? string.Empty,
            lesson.EstimatedMinutes,
            lesson.Quiz is not null);
}
