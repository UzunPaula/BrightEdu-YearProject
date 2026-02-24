using BrightEdu.Application.DTOs;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.DesignPatterns.FactoryMethod.Steps;

public interface ILessonStepCreator
{
    string Type { get; }
    LessonStep Create(CreateLessonStepRequestDto dto);
}