using BrightEdu.Application.DTOs;
using BrightEdu.Application.Features.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightEdu.Web.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
public class AdminQuizzesController : ControllerBase
{
    private readonly IAdminQuizService _service;

    public AdminQuizzesController(IAdminQuizService service)
    {
        _service = service;
    }

    [HttpGet("api/admin/lessons/{lessonId:guid}/quiz")]
    public async Task<ActionResult<AdminQuizDto>> GetByLessonId(Guid lessonId, CancellationToken ct)
    {
        var quiz = await _service.GetByLessonIdAsync(lessonId, ct);
        return quiz is null ? NotFound() : Ok(quiz);
    }

    [HttpGet("api/admin/modules/{moduleId:guid}/quiz")]
    public async Task<ActionResult<AdminQuizDto>> GetByModuleId(Guid moduleId, CancellationToken ct)
    {
        var quiz = await _service.GetByModuleIdAsync(moduleId, ct);
        return quiz is null ? NotFound() : Ok(quiz);
    }

    [HttpGet("api/admin/quizzes/{id:guid}")]
    public async Task<ActionResult<AdminQuizDto>> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            var quiz = await _service.GetByIdAsync(id, ct);
            return Ok(quiz);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("api/admin/lessons/{lessonId:guid}/quiz")]
    public async Task<ActionResult<AdminQuizDto>> CreateForLesson(Guid lessonId, [FromBody] CreateQuizForLessonRequest body, CancellationToken ct)
    {
        try
        {
            var merged = body with { LessonId = lessonId };
            var quiz = await _service.CreateForLessonAsync(merged, ct);
            return CreatedAtAction(nameof(GetById), new { id = quiz.Id }, quiz);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("api/admin/modules/{moduleId:guid}/quiz")]
    public async Task<ActionResult<AdminQuizDto>> CreateForModule(Guid moduleId, [FromBody] CreateQuizForModuleRequest body, CancellationToken ct)
    {
        try
        {
            var merged = body with { ModuleId = moduleId };
            var quiz = await _service.CreateForModuleAsync(merged, ct);
            return CreatedAtAction(nameof(GetById), new { id = quiz.Id }, quiz);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("api/admin/quizzes/{id:guid}")]
    public async Task<ActionResult<AdminQuizDto>> Update(Guid id, UpdateQuizRequest request, CancellationToken ct)
    {
        try
        {
            var quiz = await _service.UpdateAsync(id, request, ct);
            return Ok(quiz);
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

    [HttpDelete("api/admin/quizzes/{id:guid}")]
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

    [HttpPost("api/admin/quizzes/{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
    {
        try
        {
            await _service.PublishAsync(id, ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
