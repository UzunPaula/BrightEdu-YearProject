namespace BrightEdu.Domain.Entities;

public class Course
{
    public Guid Id { get; private set; } // incapsulare: nu se modifică direct din exterior
    public string Title { get; private set; }
    public string? Description { get; private set; }

    private Course() { } // pentru EF Core mai târziu

    public Course(Guid id, string title, string? description)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        SetTitle(title);
        Id = id;
        Description = description;
    }

    public void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Titlul nu poate fi gol.", nameof(title));

        Title = title.Trim();
    }

    public void SetDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}