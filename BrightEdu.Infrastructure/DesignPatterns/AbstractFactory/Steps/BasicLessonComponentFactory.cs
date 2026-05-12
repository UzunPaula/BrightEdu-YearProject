using BrightEdu.Application.DesignPatterns.AbstractFactory.Steps;
using BrightEdu.Domain.Entities;
using BrightEdu.Domain.Enums;

namespace BrightEdu.Infrastructure.DesignPatterns.AbstractFactory.Steps;

// Abstract Factory concretă: creează componente pentru o lecție simplă (text + traducere RO).
public sealed class BasicLessonComponentFactory : ILessonComponentFactory
{
    public string FactoryType => "basic";

    public LessonContentBlock CreateContentBlock(Guid lessonId, int order, string content)
    {
        var configJson = $"{{\"content\":\"{Escape(content)}\"}}";
        return new LessonContentBlock(Guid.NewGuid(), lessonId, order, ContentBlockType.Text, configJson);
    }

    public LessonTranslation CreateTranslation(Guid lessonId, string title, string summary)
        => new(Guid.NewGuid(), lessonId, LanguageCode.Ro, title, summary);

    private static string Escape(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"");
}
