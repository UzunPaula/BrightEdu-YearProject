using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;

namespace BrightEdu.Application.Features.Courses;

public interface IGetCoursesService
{
    Task<IReadOnlyList<CourseDto>> GetAllAsync(CancellationToken ct = default);
}

// SRP: service-ul conține logica de use case și maparea în DTO.
// DIP: depinde de ICourseReadRepository, nu de repository concret.
public sealed class GetCoursesService : IGetCoursesService
{
    private readonly ICourseReadRepository _courseReadRepository; // ISP: doar citire

    public GetCoursesService(ICourseReadRepository courseReadRepository)
    {
        _courseReadRepository = courseReadRepository;
    }

    public async Task<IReadOnlyList<CourseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var courses = await _courseReadRepository.GetAllAsync(ct);

        // SRP: aici transform entități Domain în DTO pentru API.
        return courses.Select(c => new CourseDto(c.Id, c.Title, c.Description)).ToList();
    }
}