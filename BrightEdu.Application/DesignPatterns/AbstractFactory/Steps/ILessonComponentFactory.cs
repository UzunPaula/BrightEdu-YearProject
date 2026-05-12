using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.DesignPatterns.AbstractFactory.Steps;

// Abstract Factory: declară familia de obiecte asociate unei lecții (bloc + traducere).
public interface ILessonComponentFactory
{
    string FactoryType { get; }
    LessonContentBlock CreateContentBlock(Guid lessonId, int order, string content);
    LessonTranslation CreateTranslation(Guid lessonId, string title, string summary);
}
