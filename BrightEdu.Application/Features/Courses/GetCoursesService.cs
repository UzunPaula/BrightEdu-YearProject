using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;

namespace BrightEdu.Application.Features.Courses;

public interface IGetCoursesService
{
    Task<IReadOnlyList<CourseDto>> GetAllAsync(CancellationToken ct = default);
}

// Serviciu care citește cursurile din repository și le transformă în DTO-uri.
// Respectă SRP, fiindcă se ocupă doar de use case-ul de citire a cursurilor.
public sealed class GetCoursesService : IGetCoursesService
{
    // Repository doar pentru citire, conform ISP.
    private readonly ICourseReadRepository _courseReadRepository; 

    public GetCoursesService(ICourseReadRepository courseReadRepository)
    {
        _courseReadRepository = courseReadRepository;
    }

    public async Task<IReadOnlyList<CourseDto>> GetAllAsync(CancellationToken ct = default)
    {
        // Citim entitățile din repository.
        var courses = await _courseReadRepository.GetAllAsync(ct);

        // Mapăm entitățile Domain în DTO-uri pentru a nu expune direct modelul intern.
        IReadOnlyList<CourseDto> result = courses
            .Select(c => new CourseDto(c.Id, c.Title, c.Description))
            .ToList()
            .AsReadOnly();

        return result;
    }
}