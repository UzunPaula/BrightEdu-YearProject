using BrightEdu.Application.DesignPatterns.Adapter.Steps;
using BrightEdu.Application.DesignPatterns.Singleton;
using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;

namespace BrightEdu.Application.Features.Lessons;

public class GetTemplateAsCreationModelService : IGetTemplateAsCreationModelService
{
    private readonly ILessonTemplateRegistry _lessonTemplateRegistry;
    private readonly ITemplateLessonAdapter _templateLessonAdapter;

    public GetTemplateAsCreationModelService(
        ILessonTemplateRegistry lessonTemplateRegistry,
        ITemplateLessonAdapter templateLessonAdapter)
    {
        _lessonTemplateRegistry = lessonTemplateRegistry;
        _templateLessonAdapter = templateLessonAdapter;
    }

    public LessonCreationModel GetById(Guid templateId)
    {
        // Caut lecția template în registry după ID.
        var templateLesson = _lessonTemplateRegistry.GetTemplateById(templateId);

        // Dacă template-ul nu există, arunc excepție.
        if (templateLesson is null)
        {
            throw new InvalidOperationException("Template-ul nu a fost găsit.");
        }
        
        // Adapterul transformă lecția template în modelul comun.
        return _templateLessonAdapter.Adapt(templateLesson);
    }
}