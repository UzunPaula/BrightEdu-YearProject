using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Features.Lessons.Mapper;

// OCP: mapper separat pentru QuestionStep.
public sealed class QuestionStepDtoMapper : ILessonStepDtoMapper
{
    public string Type => "question";
    public bool CanMap(LessonStep step) => step is QuestionStep;

    public LessonStepDto Map(LessonStep step)
    {
        var s = (QuestionStep)step;
        return new QuestionStepDto(s.Id, s.Order, s.QuestionText, s.Options, s.CorrectOptionIndex);
    }
}