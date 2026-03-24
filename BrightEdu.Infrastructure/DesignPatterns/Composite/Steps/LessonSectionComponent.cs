using BrightEdu.Application.DesignPatterns.Composite.Steps;

namespace BrightEdu.Infrastructure.DesignPatterns.Composite.Steps;

// Reprezintă o secțiune din lecție, care poate conține alte componente.
public class LessonSectionComponent : ILessonComponent
{
    // Lista elementelor din această secțiune.
    private readonly List<ILessonComponent> _children = new();

    public string Title { get; }

    public LessonSectionComponent(string title)
    {
        Title = title;
    }

    // Adaugă un step sau o altă secțiune în secțiunea curentă.
    public void Add(ILessonComponent component)
    {
        _children.Add(component);
    }

    // Elimină o componentă din secțiune.
    public void Remove(ILessonComponent component)
    {
        _children.Remove(component);
    }

    public void Display(int level = 0)
    {
        // Afișează secțiunea curentă.
        Console.WriteLine(new string(' ', level * 2) + $"Section: {Title}");

        // Afișează toate componentele din interiorul secțiunii.
        foreach (var child in _children)
        {
            child.Display(level + 1);
        }
    }

    // Returnează toate componentele conținute în secțiune.
    public IReadOnlyList<ILessonComponent> GetChildren()
    {
        return _children;
    }
}