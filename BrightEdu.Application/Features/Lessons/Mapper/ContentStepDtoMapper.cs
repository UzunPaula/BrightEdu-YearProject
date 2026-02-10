using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Features.Lessons.Mapper;

// OCP: mapper separat pentru ContentStep, nu modific service-ul când apar tipuri noi.
public sealed class ContentStepDtoMapper : ILessonStepDtoMapper
{
    public bool CanMap(LessonStep step) => step is ContentStep;

    public LessonStepDto Map(LessonStep step)
    {
        var s = (ContentStep)step;
        return new ContentStepDto(s.Id, s.Order, s.Content);
    }
}