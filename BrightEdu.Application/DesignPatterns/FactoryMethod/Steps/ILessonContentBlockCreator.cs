using BrightEdu.Domain.Entities;
using BrightEdu.Domain.Enums;

namespace BrightEdu.Application.DesignPatterns.FactoryMethod.Steps;

// Factory Method: declară operația de fabricare fără a cunoaște tipul concret al blocului.
public interface ILessonContentBlockCreator
{
    ContentBlockType BlockType { get; }
    LessonContentBlock Create(Guid lessonId, int order, string configJson);
}
