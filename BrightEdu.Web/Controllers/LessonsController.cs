using BrightEdu.Application.DTOs;
using BrightEdu.Application.Features.Lessons;
using Microsoft.AspNetCore.Mvc;

namespace BrightEdu.Web.Controllers;

// SRP: controllerul doar primește request, validează minim (ex: NotFound) și returnează response.
// DIP: depinde de IGetLessonService, nu de GetLessonService.
[ApiController]
[Route("api/[controller]")]
public class LessonsController : ControllerBase
{
    private readonly IGetLessonService _service;

    public LessonsController(IGetLessonService service)
    {
        _service = service;
    }

    [HttpGet("sample")]
    public async Task<ActionResult<LessonDto>> GetSample(CancellationToken ct)
    {
        // doar pentru demo, ca să testez fără să introduc mereu GUID în Swagger
        var id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var lesson = await _service.GetByIdAsync(id, ct);
        if (lesson is null) return NotFound();
        return Ok(lesson);
    }
}