using BrightEdu.Application.DTOs;
using BrightEdu.Application.Features.Courses;
using Microsoft.AspNetCore.Mvc;

namespace BrightEdu.Web.Controllers;

// SRP(Single Resposibility Principle): controller doar expune API sau endpointuri HTTP și cheamă service-ul, nu contine logica de business.
// DIP: depinde de interfața IGetCoursesService, nu de o implementare concretă.
[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly IGetCoursesService _service; // DIP: depinde de interfață

    public CoursesController(IGetCoursesService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CourseDto>>> GetAll(CancellationToken ct)
    {
        // SRP: delegă obținerea datelor către layer-ul Application.
        var result = await _service.GetAllAsync(ct);
        return Ok(result);
    }
}