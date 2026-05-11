using System.Security.Claims;
using BrightEdu.Application.DTOs;
using BrightEdu.Application.Features.Students;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightEdu.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/me")]
public class StudentController : ControllerBase
{
    private readonly IStudentDashboardService _studentDashboardService;
    private readonly ILessonProgressService _lessonProgressService;

    public StudentController(
        IStudentDashboardService studentDashboardService,
        ILessonProgressService lessonProgressService)
    {
        _studentDashboardService = studentDashboardService;
        _lessonProgressService = lessonProgressService;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<StudentDashboardDto>> GetDashboard(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        return Ok(await _studentDashboardService.GetAsync(userId, ct));
    }

    [HttpPost("lessons/{lessonId:guid}/opened")]
    public async Task<ActionResult<LessonProgressDto>> MarkLessonOpened(Guid lessonId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        try
        {
            return Ok(await _lessonProgressService.MarkOpenedAsync(userId, lessonId, ct));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("lessons/progress")]
    public async Task<ActionResult<LessonProgressDto>> UpdateLessonProgress(UpdateLessonProgressRequestDto request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        try
        {
            return Ok(await _lessonProgressService.UpdateAsync(userId, request, ct));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private bool TryGetUserId(out Guid userId)
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(raw, out userId);
    }
}
