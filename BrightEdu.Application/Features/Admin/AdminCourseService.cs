using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Domain.Enums;

namespace BrightEdu.Application.Features.Admin;

public interface IAdminCourseService
{
    Task<IReadOnlyList<AdminCourseDto>> GetAllAsync(CancellationToken ct = default);
    Task<AdminCourseDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<AdminCourseDto> CreateAsync(CreateCourseRequest request, CancellationToken ct = default);
    Task<AdminCourseDto?> UpdateAsync(Guid id, UpdateCourseRequest request, CancellationToken ct = default);
    Task<bool> PublishAsync(Guid id, CancellationToken ct = default);
    Task<bool> ArchiveAsync(Guid id, CancellationToken ct = default);
}

public sealed class AdminCourseService : IAdminCourseService
{
    private readonly IAdminCourseRepository _repository;

    public AdminCourseService(IAdminCourseRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<AdminCourseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var courses = await _repository.GetAllAsync(ct);
        return courses.Select(Map).ToList().AsReadOnly();
    }

    public async Task<AdminCourseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var course = await _repository.GetByIdAsync(id, ct);
        return course is null ? null : Map(course);
    }

    public async Task<AdminCourseDto> CreateAsync(CreateCourseRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Titlul cursului este obligatoriu.");

        var course = new Course(Guid.NewGuid(), request.Title, request.ShortDescription);
        course.SetLevel(string.IsNullOrWhiteSpace(request.Level) ? "Beginner" : request.Level);

        if (!string.IsNullOrWhiteSpace(request.FullDescription))
            course.AddTranslation(LanguageCode.Ro, request.Title, request.ShortDescription, request.FullDescription);

        await _repository.AddAsync(course, ct);
        await _repository.SaveAsync(ct);

        return Map(course);
    }

    public async Task<AdminCourseDto?> UpdateAsync(Guid id, UpdateCourseRequest request, CancellationToken ct = default)
    {
        var course = await _repository.GetByIdAsync(id, ct);
        if (course is null) return null;

        course.AddTranslation(LanguageCode.Ro, request.Title, request.ShortDescription, request.FullDescription);
        course.SetLevel(string.IsNullOrWhiteSpace(request.Level) ? "Beginner" : request.Level);

        await _repository.SaveAsync(ct);
        return Map(course);
    }

    public async Task<bool> PublishAsync(Guid id, CancellationToken ct = default)
    {
        var course = await _repository.GetByIdAsync(id, ct);
        if (course is null) return false;

        course.SetState(CourseState.Published);
        await _repository.SaveAsync(ct);
        return true;
    }

    public async Task<bool> ArchiveAsync(Guid id, CancellationToken ct = default)
    {
        var course = await _repository.GetByIdAsync(id, ct);
        if (course is null) return false;

        course.SetState(CourseState.Archived);
        await _repository.SaveAsync(ct);
        return true;
    }

    private static AdminCourseDto Map(Course course)
    {
        var translation = course.GetTranslation(LanguageCode.Ro) ?? course.Translations.FirstOrDefault();
        return new AdminCourseDto(
            course.Id,
            course.Slug,
            course.Level,
            translation?.Title ?? course.Slug,
            translation?.ShortDescription,
            translation?.FullDescription,
            course.State.ToString(),
            course.IsPublished,
            course.Modules.Count,
            course.Lessons.Count,
            course.CreatedAt,
            course.UpdatedAt);
    }
}
