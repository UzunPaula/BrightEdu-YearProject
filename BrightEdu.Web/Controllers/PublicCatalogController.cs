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
    public async Task<ActionResult<IReadOnlyList<CourseCardDto>>> GetCourses([FromQuery] string lang = "ro", CancellationToken ct = default)
    {
        return Ok(await _courseCatalogService.GetCoursesAsync(lang, ct));
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<CourseDetailsDto>> GetCourseBySlug(string slug, [FromQuery] string lang = "ro", CancellationToken ct = default)
    {
        var result = await _courseCatalogService.GetBySlugAsync(slug, lang, ct);
        return result is null ? NotFound() : Ok(result);
    }
}
