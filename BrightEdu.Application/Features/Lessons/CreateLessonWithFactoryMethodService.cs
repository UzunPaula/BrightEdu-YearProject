using BrightEdu.Application.DesignPatterns.FactoryMethod.Steps;
using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Features.Lessons;

public sealed class CreateLessonWithFactoryMethodService : ICreateLessonWithFactoryMethodService
{
    private readonly ILessonStepCreatorResolver _resolver;
    private readonly ILessonRepository _repository;

    public CreateLessonWithFactoryMethodService(
        ILessonStepCreatorResolver resolver,
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
            var creator = _resolver.Resolve(stepDto.Type);
            var step = creator.Create(stepDto);
            lesson.AddStep(step);
        }

        await _repository.AddAsync(lesson, ct);
    }
}