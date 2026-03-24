using BrightEdu.Application.DTOs;

namespace BrightEdu.Application.DesignPatterns.Adapter.Steps;

public interface ILessonCreationAdapter
{
    // Interfață pentru adapterul care transformă requestul API într-un model comun folosit intern de aplicație.
    LessonCreationModel Adapt(CreateLessonRequestDto source);
}