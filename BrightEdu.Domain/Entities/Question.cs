namespace BrightEdu.Domain.Entities;

public class Question
{
    // Colecție internă de răspunsuri.
    // O păstrăm privată pentru a controla adăugarea răspunsurilor doar prin metodă necesară.
    private readonly List<Answer> _answers = new();
    public Guid Id { get; private set; }
    public string Text { get; private set; }

    // Cheie străină către quiz-ul din care face parte întrebarea.
    public Guid QuizId { get; private set; }

    // Proprietate de navigare către quiz.
    public Quiz Quiz { get; private set; } = null!;

    // Lista răspunsurilor, expusă doar pentru citire.
    public IReadOnlyCollection<Answer> Answers => _answers.AsReadOnly();

    // Constructor privat necesar pentru EF Core.
    private Question() { }
    
    // Tipul intrebarii din quiz
    public string Type { get; private set; } = null!;
    
    // Constructorul principal pentru crearea unei întrebări valide.
    public Question(Guid id, string text, Guid quizId)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id invalid.", nameof(id));

        if (quizId == Guid.Empty)
            throw new ArgumentException("QuizId invalid.", nameof(quizId));

        Id = id;
        QuizId = quizId;

        SetText(text);
    }

    // Setează textul întrebării cu validare.
    public void SetText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Textul întrebării nu poate fi gol.", nameof(text));

        Text = text.Trim();
    }

    // Adaugă un răspuns la întrebare.
    public void AddAnswer(Answer answer)
    {
        if (answer is null)
            throw new ArgumentNullException(nameof(answer));

        _answers.Add(answer);
    }
}