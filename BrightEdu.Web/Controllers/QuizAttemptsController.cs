using System.Security.Claims;
using BrightEdu.Application.DTOs;
using BrightEdu.Application.Features.QuizAttempts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrightEdu.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/quizzes")]
public class QuizAttemptsController : ControllerBase
{
    private readonly IStartQuizAttemptService _startQuizAttemptService;
    private readonly ISubmitQuizAttemptService _submitQuizAttemptService;
    private readonly IQuizAttemptHistoryService _quizAttemptHistoryService;

    public QuizAttemptsController(
        IStartQuizAttemptService startQuizAttemptService,
        ISubmitQuizAttemptService submitQuizAttemptService,
        IQuizAttemptHistoryService quizAttemptHistoryService)
    {
        _startQuizAttemptService = startQuizAttemptService;
        _submitQuizAttemptService = submitQuizAttemptService;
        _quizAttemptHistoryService = quizAttemptHistoryService;
    }

    [HttpPost("{quizId:guid}/attempts")]
    public async Task<ActionResult<QuizAttemptResultDto>> StartAttempt(Guid quizId, CancellationToken ct)
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdRaw, out var userId))
            return Unauthorized();

        try
        {
            return Ok(await _startQuizAttemptService.StartAsync(quizId, userId, ct));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("attempts/{attemptId:guid}/submit")]
    public async Task<ActionResult<SubmitQuizAttemptResultDto>> SubmitAttempt(
        Guid attemptId,
        SubmitQuizAttemptRequestDto request,
        CancellationToken ct)
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdRaw, out var userId))
            return Unauthorized();

        if (attemptId != request.AttemptId)
            return BadRequest(new { message = "Attempt id mismatch." });

        try
        {
            return Ok(await _submitQuizAttemptService.SubmitAsync(userId, request, ct));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{quizId:guid}/attempts/history")]
    public async Task<ActionResult<IReadOnlyList<QuizHistoryItemDto>>> GetHistory(Guid quizId, CancellationToken ct)
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdRaw, out var userId))
            return Unauthorized();

        return Ok(await _quizAttemptHistoryService.GetForStudentAsync(quizId, userId, ct));
    }
}
