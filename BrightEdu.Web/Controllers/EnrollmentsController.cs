using System.Security.Claims;
using BrightEdu.Application.DTOs;
using BrightEdu.Application.Features.Students;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightEdu.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/courses")]
public class EnrollmentsController : ControllerBase
{
    private readonly ICourseEnrollmentService _courseEnrollmentService;

    public EnrollmentsController(ICourseEnrollmentService courseEnrollmentService)
    {
        _courseEnrollmentService = courseEnrollmentService;
    }

    [HttpPost("{courseId:guid}/enroll")]
    public async Task<ActionResult<EnrollmentResultDto>> Enroll(Guid courseId, CancellationToken ct)
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdRaw, out var userId))
            return Unauthorized();

        try
        {
            return Ok(await _courseEnrollmentService.EnrollAsync(userId, courseId, ct));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{courseId:guid}/enrollment-status")]
    public async Task<ActionResult<EnrollmentStatusDto>> GetStatus(Guid courseId, CancellationToken ct)
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdRaw, out var userId))
            return Unauthorized();

        return Ok(await _courseEnrollmentService.GetStatusAsync(userId, courseId, ct));
    }

    [HttpDelete("{courseId:guid}/enroll")]
    public async Task<ActionResult<EnrollmentStatusDto>> Unenroll(Guid courseId, CancellationToken ct)
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdRaw, out var userId))
            return Unauthorized();

        return Ok(await _courseEnrollmentService.UnenrollAsync(userId, courseId, ct));
    }
}
