using BrightEdu.Application.DTOs;
using BrightEdu.Application.Features.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightEdu.Web.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/public/courses")]
public class PublicCatalogController : ControllerBase
{
    private readonly IPublicCourseCatalogService _courseCatalogService;

    public PublicCatalogController(IPublicCourseCatalogService courseCatalogService)
    {
        _courseCatalogService = courseCatalogService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CourseCardDto>>> GetCourses(CancellationToken ct)
    {
        return Ok(await _courseCatalogService.GetCoursesAsync(ct));
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<CourseDetailsDto>> GetCourseBySlug(string slug, CancellationToken ct)
    {
        var result = await _courseCatalogService.GetBySlugAsync(slug, ct);
        return result is null ? NotFound() : Ok(result);
    }
}
