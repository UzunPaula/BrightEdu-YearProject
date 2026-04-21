using BrightEdu.Application.DesignPatterns.Decorator;
using BrightEdu.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BrightEdu.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizEvaluationController : ControllerBase
{
    private readonly IQuizEvaluationService _service;

    public QuizEvaluationController(IQuizEvaluationService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<QuizEvaluationResultDto>> Evaluate(
        EvaluateQuizRequestDto request,
        CancellationToken ct)
    {
        try
        {
            var result = await _service.EvaluateAsync(request, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}