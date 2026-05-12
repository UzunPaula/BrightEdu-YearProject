using BrightEdu.Domain.Enums;

namespace BrightEdu.Domain.Entities;

public class Quiz
{
    private readonly List<Question> _questions = new();
    private readonly List<QuizAttempt> _attempts = new();

    private Quiz()
    {
    }

    private Quiz(Guid id, string title, Guid? lessonId, Guid? moduleId)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));

        Id = id;
        LessonId = lessonId;
        ModuleId = moduleId;
        Title = string.IsNullOrWhiteSpace(title) ? throw new ArgumentException("Titlul quiz-ului nu poate fi gol.", nameof(title)) : title.Trim();
        State = QuizState.Draft;
        PassingScore = 60;
        MaxAttempts = 1;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // Backward-compatible public constructor
    public Quiz(Guid id, string title, Guid lessonId)
        : this(id, title, lessonId, null)
    {
    }

    public static Quiz ForLesson(string title, Guid lessonId) => new Quiz(Guid.NewGuid(), title, lessonId, null);
    public static Quiz ForModule(string title, Guid moduleId) => new Quiz(Guid.NewGuid(), title, null, moduleId);

    public Guid Id { get; private set; }
    public string Title { get; private set; } = null!;
    public Guid? LessonId { get; private set; }
    public Lesson? Lesson { get; private set; }
    public Guid? ModuleId { get; private set; }
    public Module? Module { get; private set; }
    public QuizState State { get; private set; }
    public int PassingScore { get; private set; }
    public int MaxAttempts { get; private set; }
    public bool ShuffleQuestions { get; private set; }
    public bool ShuffleAnswers { get; private set; }
    public bool ShowMistakesAfterAttempt { get; private set; }
    public bool ShowOnlyWrongAnswers { get; private set; }
    public bool ShowCorrectAnswer { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();
    public IReadOnlyCollection<QuizAttempt> Attempts => _attempts.AsReadOnly();

    public void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Titlul quiz-ului nu poate fi gol.", nameof(title));

        Title = title.Trim();
        Touch();
    }

    public void SetState(QuizState state)
    {
        State = state;
        Touch();
    }

    public void ConfigureRules(int passingScore, int maxAttempts, bool shuffleQuestions, bool shuffleAnswers,
        bool showMistakesAfterAttempt = false, bool showOnlyWrongAnswers = false, bool showCorrectAnswer = false)
    {
        if (passingScore < 0 || passingScore > 100)
            throw new ArgumentOutOfRangeException(nameof(passingScore));

        if (maxAttempts < 0)
            throw new ArgumentOutOfRangeException(nameof(maxAttempts));

        PassingScore = passingScore;
        MaxAttempts = maxAttempts;
        ShuffleQuestions = shuffleQuestions;
        ShuffleAnswers = shuffleAnswers;
        ShowMistakesAfterAttempt = showMistakesAfterAttempt;
        ShowOnlyWrongAnswers = showOnlyWrongAnswers;
        ShowCorrectAnswer = showCorrectAnswer;
        Touch();
    }

    public void AddQuestion(Question question)
    {
        ArgumentNullException.ThrowIfNull(question);
        _questions.Add(question);
        Touch();
    }

    private void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
