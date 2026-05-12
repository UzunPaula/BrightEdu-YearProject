using BrightEdu.Application.DesignPatterns.Builder;
using BrightEdu.Application.DesignPatterns.Facade.Steps;
using BrightEdu.Application.DesignPatterns.Singleton;
using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;

namespace BrightEdu.Infrastructure.DesignPatterns.Facade.Steps;

// Fațada oferă un punct unic de acces pentru operațiile principale ale lecției.
// Folosește Builder pentru crearea lecțiilor și Singleton pentru accesul la template-uri.
public class LessonFacade : ILessonFacade
{
    private readonly ILessonBuilder _builder;
    private readonly ILessonTemplateRegistry _registry;
    private readonly ILessonRepository _lessonRepository;

    public LessonFacade(
        ILessonBuilder builder,
        ILessonTemplateRegistry registry,
        ILessonRepository lessonRepository)
    {
        _builder = builder;
        _registry = registry;
        _lessonRepository = lessonRepository;
    }

    public async Task<LessonDto> CreateLessonFromRequestAsync(CreateLessonRequestDto request)
    {
        // Builder — construiește lecția pas cu pas din request
        var lesson = _builder
            .WithId(Guid.NewGuid())
            .WithTitle(request.Title)
            .WithCourseId(request.CourseId)
            .WithOrder(request.Order)
            .WithContent(request.Content)
            .Build();

        _builder.Reset();

        await _lessonRepository.AddAsync(lesson);

        return new LessonDto(lesson.Id, lesson.Title, request.Content, request.Order, request.CourseId, null);
    }

    public LessonCreationModel GetLessonCreationModelFromTemplate(Guid templateId)
    {
        // Singleton — accesează registry-ul central de template-uri
        var template = _registry.GetTemplateById(templateId)
            ?? throw new InvalidOperationException("Template-ul nu a fost găsit.");

        // Builder — convertește template-ul într-un model de creare
        return new LessonCreationModel
        {
            Id = template.Id,
            Title = template.Title,
            Steps = template.ContentBlocks
                .OrderBy(b => b.Order)
                .Select(b => new LessonCreationStepModel
                {
                    Id = b.Id,
                    Type = "content",
                    Order = b.Order,
                    Content = b.ConfigJson
                })
                .ToList()
        };
    }
}
