using BrightEdu.Domain.Enums;

namespace BrightEdu.Application.DesignPatterns.FactoryMethod.Steps;

// Rezolvatorul selectează creator-ul potrivit pentru tipul de bloc cerut.
public interface ILessonContentBlockCreatorResolver
{
    ILessonContentBlockCreator Resolve(ContentBlockType blockType);
}
