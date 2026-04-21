namespace BrightEdu.Domain.Entities;

public class Course
{
    // Colecție internă de lecții. O păstrez privată ca să controlez modificările doar din interiorul entității.
    private readonly List<Lesson> _lessons = new();
    public Guid Id { get; private set; } // incapsulare: nu se modifică direct din exterior
    public string Title { get; private set; }
    public string? Description { get; private set; }
    
    // Expunem lecțiile doar pentru citire.
    // Astfel, codul din exterior nu poate modifica direct lista.
    public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();
    
    // Constructor privat necesar pentru EF Core care îl folosește la maparea entității din baza de date.
    private Course() { } 

    // Constructorul principal folosit la crearea unui curs valid.
    public Course(Guid id, string title, string? description)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));

        Id = id;
        
        // Folosim metode separate pentru a păstra validările într-un singur loc.
        SetTitle(title);
        SetDescription(description);
    }

    // Setează titlul cursului cu validare.
    public void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Titlul nu poate fi gol.", nameof(title));

        Title = title.Trim();
    }

    // Setează descrierea cursului. Dacă textul este gol sau conține doar spații, salvăm null.
    public void SetDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}