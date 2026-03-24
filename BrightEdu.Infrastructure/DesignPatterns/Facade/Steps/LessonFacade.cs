using BrightEdu.Application.DesignPatterns.Facade.Steps;
using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;

namespace BrightEdu.Infrastructure.DesignPatterns.Facade.Steps;

// Fațada oferă un punct unic de acces pentru operațiile principale ale lecției.
public class LessonFacade : ILessonFacade
{
    private readonly ICreateLessonWithAdapterService _createLessonWithAdapterService;
    private readonly IGetTemplateAsCreationModelService _getTemplateAsCreationModelService;

    public LessonFacade(
        ICreateLessonWithAdapterService createLessonWithAdapterService,
        IGetTemplateAsCreationModelService getTemplateAsCreationModelService)
    {
        _createLessonWithAdapterService = createLessonWithAdapterService;
        _getTemplateAsCreationModelService = getTemplateAsCreationModelService;
    }

    public async Task<LessonDto> CreateLessonFromRequestAsync(CreateLessonRequestDto request)
    {
        // Creează lecția din request prin serviciul intern corespunzător.
        return await _createLessonWithAdapterService.CreateAsync(request);
    }

    public LessonCreationModel GetLessonCreationModelFromTemplate(Guid templateId)
    {
        // Obține modelul comun al lecției pornind de la un template.
        return _getTemplateAsCreationModelService.GetById(templateId);
    }
}