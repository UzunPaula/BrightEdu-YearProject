namespace BrightEdu.Domain.Entities;

public class Quiz
{
    // Colecție internă de întrebări.
    // O păstrăm privată pentru a controla adăugarea întrebărilor doar prin metodă dedicată.
    private readonly List<Question> _questions = new();
    public Guid Id { get; private set; }
    public string Title { get; private set; }

    // Cheie străină către lecția de care aparține quiz-ul.
    public Guid LessonId { get; private set; }

    // Proprietate de navigare către lecție.
    public Lesson Lesson { get; private set; } = null!;

    // Lista întrebărilor din quiz, expusă doar pentru citire.
    public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

    // Constructor privat necesar pentru EF Core.
    private Quiz() { }

    // Constructorul principal pentru crearea unui quiz valid.
    public Quiz(Guid id, string title, Guid lessonId)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id invalid.", nameof(id));

        if (lessonId == Guid.Empty)
            throw new ArgumentException("LessonId invalid.", nameof(lessonId));

        Id = id;
        LessonId = lessonId;

        SetTitle(title);
    }

    // Setează titlul quiz-ului cu validare.
    public void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Titlul quiz-ului nu poate fi gol.", nameof(title));

        Title = title.Trim();
    }

    // Adaugă o întrebare în quiz.
    public void AddQuestion(Question question)
    {
        if (question is null)
            throw new ArgumentNullException(nameof(question));

        _questions.Add(question);
    }
}