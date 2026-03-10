using BrightEdu.Application.DesignPatterns.AbstractFactory.Steps;
using BrightEdu.Application.DTOs;
using BrightEdu.Application.Features.Lessons.Mapper;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Infrastructure.DesignPatterns.FactoryMethod.Steps;

namespace BrightEdu.Infrastructure.DesignPatterns.AbstractFactory.Steps;

// Fabrica concretă pentru step-uri de tip "question".
// Rol: recunoaște tipul și creează QuestionStep + mapper-ul potrivit.
public sealed class QuestionLessonStepFactory : ILessonStepAbstractFactory
{
    // Tipul suportat de această fabrică.
    private const string Type = "question";

    // Creatorul instanțiază obiectul concret.
    // Mapper-ul convertește entity -> DTO pentru afișare/response.
    private readonly QuestionStepCreator _creator;
    private readonly QuestionStepDtoMapper _mapper;

    // Injectez dependențele prin constructor ca să respect DIP și să testez ușor.
    public QuestionLessonStepFactory(
        QuestionStepCreator creator,
        QuestionStepDtoMapper mapper)
    {
        _creator = creator;
        _mapper = mapper;
    }

    // Spune dacă această fabrică poate gestiona stepType primit.
    // Folosesc comparație case-insensitive ca să evit erori din input.
    public bool CanHandle(string stepType)
        => string.Equals(stepType, Type, StringComparison.OrdinalIgnoreCase);

    // Creează step-ul concret pe baza request-ului.
    // Nu instanțiez direct, dar deleg către QuestionStepCreator.
    public LessonStep CreateStep(CreateLessonStepRequestDto request)
        => _creator.Create(request);

    // Returnez mapper-ul corect pentru step-ul de tip question.
    public ILessonStepDtoMapper CreateMapper()
        => _mapper;
}