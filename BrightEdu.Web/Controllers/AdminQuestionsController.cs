using BrightEdu.Application.DTOs;
using BrightEdu.Application.Features.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightEdu.Web.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
public class AdminQuestionsController : ControllerBase
{
    private readonly IAdminQuestionService _service;

    public AdminQuestionsController(IAdminQuestionService service)
    {
        _service = service;
    }

    [HttpGet("api/admin/quizzes/{quizId:guid}/questions")]
    public async Task<ActionResult<IReadOnlyList<AdminQuestionDto>>> GetByQuizId(Guid quizId, CancellationToken ct)
        => Ok(await _service.GetByQuizIdAsync(quizId, ct));

    [HttpPost("api/admin/quizzes/{quizId:guid}/questions")]
    public async Task<ActionResult<AdminQuestionDto>> Create(Guid quizId, [FromBody] CreateQuestionRequest body, CancellationToken ct)
    {
        try
        {
            var merged = body with { QuizId = quizId };
            var question = await _service.CreateAsync(merged, ct);
            return Ok(question);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("api/admin/questions/{id:guid}")]
    public async Task<ActionResult<AdminQuestionDto>> Update(Guid id, UpdateQuestionRequest request, CancellationToken ct)
    {
        try
        {
            var question = await _service.UpdateAsync(id, request, ct);
            return Ok(question);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("api/admin/questions/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            await _service.DeleteAsync(id, ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
