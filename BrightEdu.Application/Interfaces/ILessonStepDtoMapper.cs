using BrightEdu.Application.DTOs;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Interfaces;

// OCP: adaug un mapper nou pentru un step nou, fără să modific GetLessonService.
public interface ILessonStepDtoMapper
{
    bool CanMap(LessonStep step);
    LessonStepDto Map(LessonStep step);
}