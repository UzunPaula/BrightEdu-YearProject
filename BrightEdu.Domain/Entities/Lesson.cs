using BrightEdu.Domain.Prototype;

namespace BrightEdu.Domain.Entities;

// Entitatea principală care reprezintă o lecție în sistem.
// Implementează IPrototype pentru a permite duplicarea lecțiilor.
public class Lesson : IPrototype<Lesson>
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }

    private readonly List<LessonStep> _steps = new();
    public IReadOnlyList<LessonStep> Steps => _steps.AsReadOnly();  // Listă privată pentru a controla modul de adăugare

    // Constructor cu validări pentru a preveni stări invalide.
    public Lesson(Guid id, string title)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Titlul nu poate fi gol.", nameof(title));

        Id = id;
        Title = title.Trim();
    }
    
    // Adaugă un pas în lecție și menține ordinea sortată.
    public void AddStep(LessonStep step)
    {
        if (step is null) throw new ArgumentNullException(nameof(step));

        // Prevenim duplicate după Order
        if (_steps.Any(s => s.Order == step.Order))
            throw new InvalidOperationException("Există deja un step cu acest Order.");

        _steps.Add(step);
        _steps.Sort((a, b) => a.Order.CompareTo(b.Order));
    }
    
    // Implementare Prototype - creează o copie a lecției.
    public Lesson Clone()
    {
        // Generăm ID nou pentru copie
        var clonedLesson = new Lesson(
            Guid.NewGuid(),
            $"{Title} Copy"); // SuFix pentru identificare ușoară

        foreach (var step in _steps.OrderBy(s => s.Order))
        {
            clonedLesson.AddStep(step.Clone()); // Aici fac deep copy
        }

        return clonedLesson;
    }
}
