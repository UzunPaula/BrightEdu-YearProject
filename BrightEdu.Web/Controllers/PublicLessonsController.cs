using BrightEdu.Application.DTOs;
using BrightEdu.Application.Features.Lessons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightEdu.Web.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/public/lessons")]
public class PublicLessonsController : ControllerBase
{
    private readonly IPublicLessonService _publicLessonService;

    public PublicLessonsController(IPublicLessonService publicLessonService)
    {
        _publicLessonService = publicLessonService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LessonDetailsDto>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _publicLessonService.GetByIdAsync(id, ct);
        return result is null ? NotFound() : Ok(result);
    }
}
