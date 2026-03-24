using BrightEdu.Application.DesignPatterns.Facade.Steps;
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
    private readonly ICreateLessonWithBuilderService _createWithBuilder;
    private readonly ICreateLessonWithPrototypeService _createWithPrototype;
    private readonly ICreateLessonFromTemplateService _createFromTemplate;
    private readonly ICreateLessonWithAdapterService _createLessonWithAdapterService;
    private readonly IGetTemplateAsCreationModelService _getTemplateAsCreationModelService;
    private readonly IBuildLessonCompositeService _buildLessonCompositeService;
    private readonly ILessonFacade _lessonFacade;

    public LessonsController(
        IGetLessonService getService,
        ICreateLessonWithFactoryMethodService createWithFactoryMethod,
        ICreateLessonWithAbstractFactoryService createWithAbstractFactory,
        ICreateLessonWithBuilderService createWithBuilder,
        ICreateLessonWithPrototypeService createWithPrototype,
        ICreateLessonFromTemplateService createFromTemplate,
        ICreateLessonWithAdapterService createLessonWithAdapterService,
        IGetTemplateAsCreationModelService getTemplateAsCreationModelService,
        IBuildLessonCompositeService buildLessonCompositeService,
        ILessonFacade lessonFacade)
    {
        _getService = getService;
        _createWithFactoryMethod = createWithFactoryMethod;
        _createWithAbstractFactory = createWithAbstractFactory;
        _createWithBuilder = createWithBuilder;
        _createWithPrototype = createWithPrototype;
        _createFromTemplate = createFromTemplate;
        _createLessonWithAdapterService = createLessonWithAdapterService;
        _getTemplateAsCreationModelService = getTemplateAsCreationModelService;
        _buildLessonCompositeService = buildLessonCompositeService;
        _lessonFacade = lessonFacade;
    }

    // Endpoint de test - returnează o lecție cu ID fix
    [HttpGet("sample")]
    public async Task<ActionResult<LessonDto>> GetSample(CancellationToken ct)
    {
        // doar pentru demo, ca să testez fără să introduc mereu GUID în Swagger
        var id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var lesson = await _getService.GetByIdAsync(id, ct);
        if (lesson is null) return NotFound();
        return Ok(lesson);
    }

    // Endpoint care folosește Factory Method
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

    // Endpoint care folosește Abstract Factory
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
    
    // Endpoint care folosește Builder
    [HttpPost("builder")]
    public async Task<ActionResult<LessonDto>> CreateBuilder(
        [FromBody] CreateLessonRequestDto dto,
        CancellationToken ct)
    {
        await _createWithBuilder.CreateAsync(dto, ct);

        var created = await _getService.GetByIdAsync(dto.Id, ct);
        if (created is null) return Problem("Lesson was not saved.");

        return Ok(created);
    }
    
    // Endpoint care folosește Prototype (clonează o lecție existentă)
    [HttpPost("prototype/{sourceLessonId:guid}")]
    public async Task<ActionResult<LessonDto>> CreatePrototype(
        Guid sourceLessonId,
        CancellationToken ct)
    {
        var newLessonId = await _createWithPrototype.CreateFromPrototypeAsync(sourceLessonId, ct);

        var created = await _getService.GetByIdAsync(newLessonId, ct);
        if (created is null) return Problem("Lesson was not saved.");

        return Ok(created);
    }
    
    // Endpoint care folosește Singleton + Prototype (creează din template)
    [HttpPost("template/{templateId:guid}")]
    public async Task<ActionResult<LessonDto>> CreateFromTemplate(
        Guid templateId,
        CancellationToken ct)
    {
        var newLessonId = await _createFromTemplate.CreateFromTemplateAsync(templateId, ct);

        var created = await _getService.GetByIdAsync(newLessonId, ct);
        if (created is null) return Problem("Lesson was not saved.");

        return Ok(created);
    }
    
    // Endpoint care folosește Adapter
    [HttpPost("adapter")]
    public async Task<ActionResult<LessonDto>> CreateWithAdapter([FromBody] CreateLessonRequestDto request)
    {
        var lesson = await _createLessonWithAdapterService.CreateAsync(request);
        return Ok(lesson);
    }
    [HttpGet("adapter/template/{templateId:guid}")]
    public ActionResult<LessonCreationModel> GetTemplateAsCreationModel(Guid templateId)
    {
        var result = _getTemplateAsCreationModelService.GetById(templateId);
        return Ok(result);
    }
    
    // Endpoint care folosește Composite
    [HttpGet("composite")]
    public ActionResult<string> GetCompositeLessonStructure()
    {
        var result = _buildLessonCompositeService.BuildSampleLessonStructure();
        return Ok(result);
    }
    
    // Endpoint care folosește Facade pentru a crea o lecție din request.
    [HttpPost("facade")]
    public async Task<ActionResult<LessonDto>> CreateWithFacade([FromBody] CreateLessonRequestDto request)
    {
        var lesson = await _lessonFacade.CreateLessonFromRequestAsync(request);
        return Ok(lesson);
    }

// Endpoint care folosește Facade pentru a obține modelul comun din template.
    [HttpGet("facade/template/{templateId:guid}")]
    public ActionResult<LessonCreationModel> GetTemplateWithFacade(Guid templateId)
    {
        var result = _lessonFacade.GetLessonCreationModelFromTemplate(templateId);
        return Ok(result);
    }
}