using BrightEdu.Application.DesignPatterns.FactoryMethod.Steps;
using BrightEdu.Domain.Entities;
using BrightEdu.Domain.Enums;

namespace BrightEdu.Infrastructure.DesignPatterns.FactoryMethod.Steps;

// Factory Method concret: creează blocuri de tip Video.
public sealed class VideoContentBlockCreator : ILessonContentBlockCreator
{
    public ContentBlockType BlockType => ContentBlockType.Video;

    public LessonContentBlock Create(Guid lessonId, int order, string configJson)
        => new(Guid.NewGuid(), lessonId, order, ContentBlockType.Video, configJson);
}
