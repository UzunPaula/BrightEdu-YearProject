using BrightEdu.Application.DesignPatterns.AbstractFactory.Steps;
//using BrightEdu.Application.DesignPatterns.FactoryMethod.Steps;
using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Features.Lessons;

public sealed class CreateLessonService : ICreateLessonService
{
    private readonly ILessonStepAbstractFactoryResolver _resolver;
    private readonly ILessonRepository _repository;
    private readonly IEnumerable<ILessonStepDtoMapper> _mappers;

    public CreateLessonService(
        ILessonStepAbstractFactoryResolver resolver,
        ILessonRepository repository,
        IEnumerable<ILessonStepDtoMapper> mappers)
    {
        _resolver = resolver;
        _repository = repository;
        _mappers = mappers;
    }

    public async Task<LessonDto> CreateAsync(CreateLessonRequestDto dto, CancellationToken ct)
    {
        var lesson = new Lesson(dto.Id, dto.Title);

        foreach (var stepDto in dto.Steps)
        {
            var factory = _resolver.Resolve(stepDto.Type);
            var step = factory.CreateStep(stepDto);
            lesson.AddStep(step);
        }

        await _repository.AddAsync(lesson, ct); 
        
        var stepDtos = lesson.Steps
            .Select(step =>
            {
                var mapper = _mappers.First(m =>
                    string.Equals(m.Type, step.Type, StringComparison.OrdinalIgnoreCase));
                return mapper.Map(step);
            })
            .ToList();

        return new LessonDto(lesson.Id, lesson.Title, stepDtos);
    }
}