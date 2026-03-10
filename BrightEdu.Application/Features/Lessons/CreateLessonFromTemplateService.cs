using BrightEdu.Application.DesignPatterns.Singleton;
using BrightEdu.Application.Interfaces;

namespace BrightEdu.Application.Features.Lessons;

public class CreateLessonFromTemplateService : ICreateLessonFromTemplateService
{
    private readonly ILessonTemplateRegistry _templateRegistry;
    private readonly ILessonRepository _lessonRepository;

    public CreateLessonFromTemplateService(
        ILessonTemplateRegistry templateRegistry,
        ILessonRepository lessonRepository)
    {
        _templateRegistry = templateRegistry;
        _lessonRepository = lessonRepository;
    }

    public async Task<Guid> CreateFromTemplateAsync(Guid templateId, CancellationToken ct)
    {
        var template = _templateRegistry.GetTemplateById(templateId);

        if (template is null)
            throw new ArgumentException("Template-ul nu a fost găsit.");

        // Clonează template-ul folosind Prototype
        var clonedLesson = template.Clone();
        
        // Salvează lecția nouă în repository
        await _lessonRepository.AddAsync(clonedLesson, ct);

        // Returnează ID-ul lecției create
        return clonedLesson.Id;
    }
}