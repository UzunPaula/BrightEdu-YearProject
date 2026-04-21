using BrightEdu.Application.DesignPatterns.Flyweight;
using BrightEdu.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BrightEdu.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionRenderingController : ControllerBase
{
    private readonly QuestionRenderingService _service;

    public QuestionRenderingController(QuestionRenderingService service)
    {
        _service = service;
    }

    // Returnează întrebarea + datele partajate din Flyweight
    [HttpGet("{questionId:guid}")]
    public async Task<ActionResult<RenderedQuestionDto>> Render(
        Guid questionId,
        CancellationToken ct)
    {
        try
        {
            var result = await _service.RenderAsync(questionId, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}