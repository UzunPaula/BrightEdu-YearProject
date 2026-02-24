using BrightEdu.Application.DTOs;
using BrightEdu.Application.Features.Lessons;
using BrightEdu.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BrightEdu.Web.Controllers;

// SRP: controllerul doar primește request, validează minim (ex: NotFound) și returnează response.
// DIP: depinde de IGetLessonService, nu de GetLessonService.
[ApiController]
[Route("api/[controller]")]
public class LessonsController : ControllerBase
{
    private readonly IGetLessonService _getService;
    private readonly ICreateLessonWithFactoryMethodService _createWithFactoryMethod;
    private readonly ICreateLessonWithAbstractFactoryService _createWithAbstractFactory;

    public LessonsController(
        IGetLessonService getService,
        ICreateLessonWithFactoryMethodService createWithFactoryMethod,
        ICreateLessonWithAbstractFactoryService createWithAbstractFactory)
    {
        _getService = getService;
        _createWithFactoryMethod = createWithFactoryMethod;
        _createWithAbstractFactory = createWithAbstractFactory;
    }

    [HttpGet("sample")]
    public async Task<ActionResult<LessonDto>> GetSample(CancellationToken ct)
    {
        // doar pentru demo, ca să testez fără să introduc mereu GUID în Swagger
        var id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var lesson = await _getService.GetByIdAsync(id, ct);
        if (lesson is null) return NotFound();
        return Ok(lesson);
    }

    [HttpPost("factory-method")]
    public async Task<ActionResult<LessonDto>> CreateFactoryMethod(
        [FromBody] CreateLessonRequestDto dto,
        CancellationToken ct)
    {
        await _createWithFactoryMethod.CreateAsync(dto, ct);

        var created = await _getService.GetByIdAsync(dto.Id, ct);
        if (created is null) return Problem("Lesson was not saved.");

        return Ok(created);
    }

    [HttpPost("abstract-factory")]
    public async Task<ActionResult<LessonDto>> CreateAbstractFactory(
        [FromBody] CreateLessonRequestDto dto,
        CancellationToken ct)
    {
        await _createWithAbstractFactory.CreateAsync(dto, ct);

        var created = await _getService.GetByIdAsync(dto.Id, ct);
        if (created is null) return Problem("Lesson was not saved.");

        return Ok(created);
    }
}