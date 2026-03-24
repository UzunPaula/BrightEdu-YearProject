using BrightEdu.Application.DTOs;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.DesignPatterns.Adapter.Steps;

public interface ITemplateLessonAdapter
{
    // Interfață pentru adapterul care transformă o lecție template într-un model comun folosit intern de aplicație.
    LessonCreationModel Adapt(Lesson source);
}