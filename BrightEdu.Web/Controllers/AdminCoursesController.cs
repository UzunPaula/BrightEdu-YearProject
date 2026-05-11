using BrightEdu.Application.DTOs;
using BrightEdu.Application.Features.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightEdu.Web.Controllers;

[ApiController]
[Route("api/admin/courses")]
[Authorize(Roles = "Admin")]
public class AdminCoursesController : ControllerBase
{
    private readonly IAdminCourseService _service;

    public AdminCoursesController(IAdminCourseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AdminCourseDto>>> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdminCourseDto>> GetById(Guid id, CancellationToken ct)
    {
        var course = await _service.GetByIdAsync(id, ct);
        return course is null ? NotFound() : Ok(course);
    }

    [HttpPost]
    public async Task<ActionResult<AdminCourseDto>> Create(CreateCourseRequest request, CancellationToken ct)
    {
        try
        {
            var course = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = course.Id }, course);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AdminCourseDto>> Update(Guid id, UpdateCourseRequest request, CancellationToken ct)
    {
        var course = await _service.UpdateAsync(id, request, ct);
        return course is null ? NotFound() : Ok(course);
    }

    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
    {
        var success = await _service.PublishAsync(id, ct);
        return success ? NoContent() : NotFound();
    }

    [HttpPost("{id:guid}/archive")]
    public async Task<IActionResult> Archive(Guid id, CancellationToken ct)
    {
        var success = await _service.ArchiveAsync(id, ct);
        return success ? NoContent() : NotFound();
    }
}
