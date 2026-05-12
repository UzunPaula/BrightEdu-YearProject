using BrightEdu.Application.DesignPatterns.AbstractFactory.Steps;
using BrightEdu.Domain.Entities;
using BrightEdu.Domain.Enums;

namespace BrightEdu.Infrastructure.DesignPatterns.AbstractFactory.Steps;

// Abstract Factory concretă: creează componente pentru o lecție multimedia (video + traducere EN).
public sealed class RichLessonComponentFactory : ILessonComponentFactory
{
    public string FactoryType => "rich";

    public LessonContentBlock CreateContentBlock(Guid lessonId, int order, string content)
    {
        var configJson = $"{{\"url\":\"{Escape(content)}\"}}";
        return new LessonContentBlock(Guid.NewGuid(), lessonId, order, ContentBlockType.Video, configJson);
    }

    public LessonTranslation CreateTranslation(Guid lessonId, string title, string summary)
        => new(Guid.NewGuid(), lessonId, LanguageCode.En, title, summary);

    private static string Escape(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"");
}
