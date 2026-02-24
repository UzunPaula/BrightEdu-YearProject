using BrightEdu.Application.DesignPatterns.AbstractFactory.Steps;
using BrightEdu.Application.DTOs;
using BrightEdu.Application.Features.Lessons.Mapper;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Infrastructure.DesignPatterns.FactoryMethod.Steps;

namespace BrightEdu.Infrastructure.DesignPatterns.AbstractFactory.Steps;

public sealed class QuestionLessonStepFactory : ILessonStepAbstractFactory
{
    private const string Type = "question";

    private readonly QuestionStepCreator _creator;
    private readonly QuestionStepDtoMapper _mapper;

    public QuestionLessonStepFactory(
        QuestionStepCreator creator,
        QuestionStepDtoMapper mapper)
    {
        _creator = creator;
        _mapper = mapper;
    }

    public bool CanHandle(string stepType)
        => string.Equals(stepType, Type, StringComparison.OrdinalIgnoreCase);

    public LessonStep CreateStep(CreateLessonStepRequestDto request)
        => _creator.Create(request);

    public ILessonStepDtoMapper CreateMapper()
        => _mapper;
}