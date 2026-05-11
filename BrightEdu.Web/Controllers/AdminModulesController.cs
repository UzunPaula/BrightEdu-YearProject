using BrightEdu.Application.DTOs;
using BrightEdu.Application.Features.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightEdu.Web.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
public class AdminModulesController : ControllerBase
{
    private readonly IAdminModuleService _service;

    public AdminModulesController(IAdminModuleService service)
    {
        _service = service;
    }

    [HttpGet("api/admin/courses/{courseId:guid}/modules")]
    public async Task<ActionResult<IReadOnlyList<AdminModuleDto>>> GetByCourse(Guid courseId, CancellationToken ct)
        => Ok(await _service.GetByCourseIdAsync(courseId, ct));

    [HttpPost("api/admin/courses/{courseId:guid}/modules")]
    public async Task<ActionResult<AdminModuleDto>> Create(Guid courseId, CreateModuleRequest request, CancellationToken ct)
    {
        try
        {
            var merged = request with { CourseId = courseId };
            var module = await _service.CreateAsync(merged, ct);
            return CreatedAtAction(nameof(GetById), new { id = module.Id }, module);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("api/admin/modules/{id:guid}")]
    public async Task<ActionResult<AdminModuleDto>> GetById(Guid id, CancellationToken ct)
    {
        var module = await _service.GetByIdAsync(id, ct);
        return module is null ? NotFound() : Ok(module);
    }

    [HttpPut("api/admin/modules/{id:guid}")]
    public async Task<ActionResult<AdminModuleDto>> Update(Guid id, UpdateModuleRequest request, CancellationToken ct)
    {
        var module = await _service.UpdateAsync(id, request, ct);
        return module is null ? NotFound() : Ok(module);
    }

    [HttpDelete("api/admin/modules/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var success = await _service.DeleteAsync(id, ct);
        return success ? NoContent() : NotFound();
    }

    [HttpGet("api/admin/modules/{id:guid}/translations")]
    public async Task<ActionResult<IReadOnlyList<EntityTranslationDto>>> GetTranslations(Guid id, CancellationToken ct)
        => Ok(await _service.GetTranslationsAsync(id, ct));

    [HttpPut("api/admin/modules/{id:guid}/translations/{lang}")]
    public async Task<IActionResult> UpsertTranslation(Guid id, string lang, UpsertModuleTranslationRequest request, CancellationToken ct)
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
