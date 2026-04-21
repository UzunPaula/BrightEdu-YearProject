using BrightEdu.Application.DTOs;
using BrightEdu.Application.Features.Lessons;
using Microsoft.AspNetCore.Mvc;

namespace BrightEdu.Web.Controllers;

// Controller simplu pentru citirea lecțiilor.
// În această etapă păstrăm doar endpointul de citire, fără pattern-urile vechi.
[ApiController]
[Route("api/[controller]")]
public class LessonsController : ControllerBase
{
    private readonly IGetLessonService _getLessonService;
    private readonly ICreateLessonService _createLessonService;

    public LessonsController(
        IGetLessonService getLessonService,
        ICreateLessonService createLessonService)
    {
        _getLessonService = getLessonService;
        _createLessonService = createLessonService;
    }

    // Endpoint de test care returnează lecția sample inserată în baza de date.
    [HttpGet("sample")]
    public async Task<ActionResult<LessonDto>> GetSample(CancellationToken ct)
    {
        var id = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var lesson = await _getLessonService.GetByIdAsync(id, ct);

        if (lesson is null)
            return NotFound();

        return Ok(lesson);
    }

    // Endpoint generic pentru a căuta o lecție după Id.
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LessonDto>> GetById(Guid id, CancellationToken ct)
    {
        var lesson = await _getLessonService.GetByIdAsync(id, ct);

        if (lesson is null)
            return NotFound();

        return Ok(lesson);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateLessonRequestDto request, CancellationToken ct)
    {
        var lessonId = await _createLessonService.CreateAsync(request, ct);

        return Ok(lessonId);
    }
}