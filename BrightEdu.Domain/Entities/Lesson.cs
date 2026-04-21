namespace BrightEdu.Domain.Entities;

public class Lesson
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Content { get; private set; }

    // Ordinea lecției în cadrul cursului.
    public int Order { get; private set; }

    // Cheie străină către cursul din care face parte lecția.
    public Guid CourseId { get; private set; }

    // Proprietate de navigare către curs.
    public Course Course { get; private set; } = null!;

    // O lecție poate avea un quiz sau poate să nu aibă deloc.
    public Quiz? Quiz { get; private set; }

    // Constructor privat necesar pentru EF Core.
    private Lesson() { }

    // Constructorul principal pentru crearea unei lecții valide.
    public Lesson(Guid id, string title, string content, int order, Guid courseId)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id invalid.", nameof(id));

        if (courseId == Guid.Empty)
            throw new ArgumentException("CourseId invalid.", nameof(courseId));

        if (order <= 0)
            throw new ArgumentException("Order trebuie să fie mai mare ca 0.", nameof(order));

        Id = id;
        CourseId = courseId;
        Order = order;

        SetTitle(title);
        SetContent(content);
    }

    // Setează titlul lecției cu validare.
    public void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Titlul lecției nu poate fi gol.", nameof(title));

        Title = title.Trim();
    }

    // Setează conținutul lecției cu validare.
    public void SetContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Conținutul lecției nu poate fi gol.", nameof(content));

        Content = content.Trim();
    }

    // Permite schimbarea ordinii lecției.
    public void SetOrder(int order)
    {
        if (order <= 0)
            throw new ArgumentException("Order trebuie să fie mai mare ca 0.", nameof(order));

        Order = order;
    }
}