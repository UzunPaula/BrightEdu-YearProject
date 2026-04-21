using BrightEdu.Application.DesignPatterns.Proxy;
using BrightEdu.Application.DTOs;
using BrightEdu.Application.Features.Quizzes;
using Microsoft.AspNetCore.Mvc;

namespace BrightEdu.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizzesController : ControllerBase
{
    private readonly ICreateQuizService _createQuizService;
    private readonly IQuizAccessService _quizAccessService;

    public QuizzesController(
        ICreateQuizService createQuizService,
        IQuizAccessService quizAccessService)
    {
        _createQuizService = createQuizService;
        _quizAccessService = quizAccessService;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateQuizRequestDto request, CancellationToken ct)
    {
        var quizId = await _createQuizService.CreateAsync(request, ct);

        return Ok(quizId);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<QuizDetailsDto>> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            var quiz = await _quizAccessService.GetByIdAsync(id, ct);

            if (quiz is null)
                return NotFound();

            return Ok(quiz);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
    }
}