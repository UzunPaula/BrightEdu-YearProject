using BrightEdu.Application.DesignPatterns.Builder;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Infrastructure.DesignPatterns.Builder;

// Implementare concretă a builder-ului pentru construirea obiectelor Lesson.
// Pattern-ul Builder este folosit pentru a separa construcția unui obiect complex de reprezentarea sa finală.
public sealed class LessonBuilder : ILessonBuilder
{
    // State-ul intern al builder-ului - stochează datele temporar până la apelul Build()
    private Guid _id;
    private string _title = string.Empty;
    private readonly List<LessonStep> _steps = new();

    // Configurează identificatorul unic al lecției.
    public ILessonBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    // Configurează titlul lecției.
    public ILessonBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    // Creează și adaugă un ContentStep în lista temporară.
    public ILessonBuilder AddContentStep(Guid id, int order, string content)
    {
        var step = new ContentStep(id, order, content);
        _steps.Add(step);
        return this;
    }

    // Creează și adaugă un QuestionStep în lista temporară.
    public ILessonBuilder AddQuestionStep(
        Guid id,
        int order,
        string questionText,
        IReadOnlyList<string> options,
        int correctOptionIndex)
    {
        var step = new QuestionStep(id, order, questionText, options, correctOptionIndex);
        _steps.Add(step);
        return this;
    }

    // Construiește obiectul final Lesson.
    // Pașii sunt sortați înainte de adăugare pentru a menține ordinea corectă.
    public Lesson Build()
    {
        var lesson = new Lesson(_id, _title);

        foreach (var step in _steps.OrderBy(s => s.Order))
        {
            lesson.AddStep(step);
        }

        return lesson;
    }

    // Resetează starea pentru reutilizarea builder-ului.
    public void Reset()
    {
        _id = Guid.Empty;
        _title = string.Empty;
        _steps.Clear();
    }
}