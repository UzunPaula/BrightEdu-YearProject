using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.DesignPatterns.AbstractFactory.Steps;

public interface ILessonStepAbstractFactory
{
    bool CanHandle(string stepType);

    LessonStep CreateStep(CreateLessonStepRequestDto dto);

    ILessonStepDtoMapper CreateMapper();
}