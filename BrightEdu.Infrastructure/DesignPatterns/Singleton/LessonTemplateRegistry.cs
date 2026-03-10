using BrightEdu.Application.DesignPatterns.Singleton;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Infrastructure.DesignPatterns.Singleton;

// Singleton - o singură instanță în întreaga aplicație
public class LessonTemplateRegistry : ILessonTemplateRegistry
{
    private readonly List<Lesson> _templates = new();

    // Returnează toate template-urile
    public IReadOnlyList<Lesson> GetAllTemplates()
    {
        return _templates.AsReadOnly();
    }

    public Lesson? GetTemplateById(Guid templateId)
    {
        return _templates.FirstOrDefault(t => t.Id == templateId);
    }

    // Adaugă un template nou
    public void AddTemplate(Lesson lesson)
    {
        if (lesson is null)
            throw new ArgumentNullException(nameof(lesson));

        // Prevenim duplicatele
        if (_templates.Any(t => t.Id == lesson.Id))
            throw new InvalidOperationException("Există deja un template cu acest Id.");

        _templates.Add(lesson);
    }
}