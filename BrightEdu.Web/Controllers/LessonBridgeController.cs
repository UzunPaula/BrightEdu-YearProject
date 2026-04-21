using BrightEdu.Application.DesignPatterns.Bridge;
using BrightEdu.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BrightEdu.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonBridgeController : ControllerBase
{
    private readonly LessonBridgeService _service;

    public LessonBridgeController(LessonBridgeService service)
    {
        _service = service;
    }

    // Client
    [HttpGet("{lessonId:guid}")]
    public async Task<ActionResult<LessonBridgeResultDto>> Render(
        Guid lessonId,
        [FromQuery] string displayType,
        [FromQuery] bool includeQuiz,
        CancellationToken ct)
    {
        try
        {
            var result = await _service.RenderAsync(lessonId, displayType, includeQuiz, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}