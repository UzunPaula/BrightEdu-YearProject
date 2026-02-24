using BrightEdu.Application.DesignPatterns.AbstractFactory.Steps;
using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Features.Lessons;

public sealed class CreateLessonWithAbstractFactoryService : ICreateLessonWithAbstractFactoryService
{
    private readonly ILessonStepAbstractFactoryResolver _resolver;
    private readonly ILessonRepository _repository;

    public CreateLessonWithAbstractFactoryService(
        ILessonStepAbstractFactoryResolver resolver,
        ILessonRepository repository)
    {
        _resolver = resolver;
        _repository = repository;
    }

    public async Task CreateAsync(CreateLessonRequestDto dto, CancellationToken ct)
    {
        var lesson = new Lesson(dto.Id, dto.Title);

        foreach (var stepDto in dto.Steps)
        {
            var factory = _resolver.Resolve(stepDto.Type);
            var step = factory.CreateStep(stepDto);
            lesson.AddStep(step);
        }

        await _repository.AddAsync(lesson, ct);
    }
}