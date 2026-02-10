namespace BrightEdu.Domain.Entities;

public class Lesson
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }

    private readonly List<LessonStep> _steps = new();
    public IReadOnlyList<LessonStep> Steps => _steps;

    private Lesson() { }

    public Lesson(Guid id, string title)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Titlul nu poate fi gol.", nameof(title));

        Id = id;
        Title = title.Trim();
    }
    
    public void AddStep(LessonStep step)
    {
        if (step is null) throw new ArgumentNullException(nameof(step));

        if (_steps.Any(s => s.Order == step.Order))
            throw new InvalidOperationException("Există deja un step cu acest Order.");

        _steps.Add(step);
        _steps.Sort((a, b) => a.Order.CompareTo(b.Order));
    }
}