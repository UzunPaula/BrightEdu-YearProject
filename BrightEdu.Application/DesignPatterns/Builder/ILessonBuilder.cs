using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.DesignPatterns.Builder;

public interface ILessonBuilder
{
    // Setează ID-ul lecției
    ILessonBuilder WithId(Guid id);
    ILessonBuilder WithTitle(string title);

    // Adaugă un pas de tip conținut
    ILessonBuilder AddContentStep(Guid id, int order, string content);

    // Adaugă un pas de tip întrebare cu opțiuni
    ILessonBuilder AddQuestionStep(
        Guid id,
        int order,
        string questionText,
        IReadOnlyList<string> options,
        int correctOptionIndex);

    Lesson Build();  // Construiește și returnează lecția finală
    void Reset(); // Resetează starea builder-ului
}