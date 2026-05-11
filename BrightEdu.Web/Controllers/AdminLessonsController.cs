using BrightEdu.Application.DTOs;
using BrightEdu.Application.Features.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightEdu.Web.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
public class AdminLessonsController : ControllerBase
{
    private readonly IAdminLessonService _service;

    public AdminLessonsController(IAdminLessonService service)
    {
        _service = service;
    }

    [HttpGet("api/admin/courses/{courseId:guid}/lessons")]
    public async Task<ActionResult<IReadOnlyList<AdminLessonDto>>> GetByCourse(Guid courseId, CancellationToken ct)
        => Ok(await _service.GetByCourseIdAsync(courseId, ct));

    [HttpPost("api/admin/courses/{courseId:guid}/lessons")]
    public async Task<ActionResult<AdminLessonDto>> Create(Guid courseId, CreateLessonRequest request, CancellationToken ct)
    {
        try
        {
            var merged = request with { CourseId = courseId };
            var lesson = await _service.CreateAsync(merged, ct);
            return CreatedAtAction(nameof(GetById), new { id = lesson.Id }, lesson);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("api/admin/lessons/{id:guid}")]
    public async Task<ActionResult<AdminLessonDto>> GetById(Guid id, CancellationToken ct)
    {
        var lesson = await _service.GetByIdAsync(id, ct);
        return lesson is null ? NotFound() : Ok(lesson);
    }

    [HttpPut("api/admin/lessons/{id:guid}")]
    public async Task<ActionResult<AdminLessonDto>> Update(Guid id, UpdateLessonRequest request, CancellationToken ct)
    {
        var lesson = await _service.UpdateAsync(id, request, ct);
        return lesson is null ? NotFound() : Ok(lesson);
    }

    [HttpDelete("api/admin/lessons/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var success = await _service.DeleteAsync(id, ct);
        return success ? NoContent() : NotFound();
    }

    [HttpPost("api/admin/lessons/{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
    {
        var success = await _service.PublishAsync(id, ct);
        return success ? NoContent() : NotFound();
    }

    [HttpGet("api/admin/lessons/{id:guid}/full")]
    public async Task<ActionResult<AdminLessonFullDto>> GetFull(Guid id, CancellationToken ct)
    {
        var lesson = await _service.GetFullAsync(id, ct);
        return lesson is null ? NotFound() : Ok(lesson);
    }

    [HttpPut("api/admin/lessons/{id:guid}/content-blocks/{lang}")]
    public async Task<ActionResult<AdminLessonFullDto>> SetContentBlocks(Guid id, string lang, SetContentBlocksRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _service.SetContentBlocksAsync(id, lang, request, ct);
            return result is null ? NotFound() : Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("api/admin/lessons/{id:guid}/attachments")]
    public async Task<ActionResult<AdminAttachmentDto>> AddAttachment(Guid id, AddAttachmentRequest request, CancellationToken ct)
    {
        var result = await _service.AddAttachmentAsync(id, request, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("api/admin/lessons/{id:guid}/attachments/link")]
    public async Task<ActionResult<AdminAttachmentDto>> AddAttachmentLink(Guid id, AddAttachmentLinkRequest request, CancellationToken ct)
    {
        var result = await _service.AddAttachmentLinkAsync(id, request, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("api/admin/lessons/{id:guid}/attachments/{attachmentId:guid}")]
    public async Task<IActionResult> RemoveAttachment(Guid id, Guid attachmentId, CancellationToken ct)
    {
        var success = await _service.RemoveAttachmentAsync(id, attachmentId, ct);
        return success ? NoContent() : NotFound();
    }

    [HttpGet("api/admin/lessons/{id:guid}/translations")]
    public async Task<ActionResult<IReadOnlyList<EntityTranslationDto>>> GetTranslations(Guid id, CancellationToken ct)
        => Ok(await _service.GetTranslationsAsync(id, ct));

    [HttpPut("api/admin/lessons/{id:guid}/translations/{lang}")]
    public async Task<IActionResult> UpsertTranslation(Guid id, string lang, UpsertLessonTranslationRequest request, CancellationToken ct)
    {
        try
        {
            var success = await _service.UpsertTranslationAsync(id, lang, request, ct);
            return success ? NoContent() : NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
