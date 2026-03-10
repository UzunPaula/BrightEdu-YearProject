using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.DesignPatterns.Singleton;

public interface ILessonTemplateRegistry
{
    // Returnează toate template-urile disponibile
    IReadOnlyList<Lesson> GetAllTemplates();
    Lesson? GetTemplateById(Guid templateId);  // Caută un template după ID
    void AddTemplate(Lesson lesson); // Adaugă un template nou în registry
}