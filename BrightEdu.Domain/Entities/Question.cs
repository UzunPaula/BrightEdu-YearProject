using BrightEdu.Domain.Enums;

namespace BrightEdu.Domain.Entities;

public class Question
{
    private readonly List<Answer> _answers = new();
    private readonly List<QuestionTranslation> _translations = new();

    private Question()
    {
    }

    public Question(Guid id, string text, Guid quizId)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        if (quizId == Guid.Empty) throw new ArgumentException("QuizId invalid.", nameof(quizId));

        Id = id;
        QuizId = quizId;
        Order = 1;
        Points = 1;
        Type = QuestionType.SingleChoice;
        ConfigJson = "{}";
        AddTranslation(LanguageCode.Ro, text, null);
    }

    public Guid Id { get; private set; }
    public Guid QuizId { get; private set; }
    public Quiz Quiz { get; private set; } = null!;
    public int Order { get; private set; }
    public int Points { get; private set; }
    public QuestionType Type { get; private set; }
    public string ConfigJson { get; private set; } = null!;

    public IReadOnlyCollection<Answer> Answers => _answers.AsReadOnly();
    public IReadOnlyCollection<QuestionTranslation> Translations => _translations.AsReadOnly();

    public string Text => GetTranslation(LanguageCode.Ro)?.Text ?? _translations.FirstOrDefault()?.Text ?? string.Empty;

    public void Configure(QuestionType type, int order, int points, string? configJson = null)
    {
        if (order <= 0) throw new ArgumentOutOfRangeException(nameof(order));
        if (points <= 0) throw new ArgumentOutOfRangeException(nameof(points));

        Type = type;
        Order = order;
        Points = points;
        ConfigJson = string.IsNullOrWhiteSpace(configJson) ? "{}" : configJson.Trim();
    }

    public void AddTranslation(LanguageCode languageCode, string text, string? explanation)
    {
        var existing = GetTranslation(languageCode);
        if (existing is null)
        {
            _translations.Add(new QuestionTranslation(Guid.NewGuid(), Id, languageCode, text, explanation));
        }
        else
        {
            existing.Update(text, explanation);
        }
    }

    public void AddAnswer(Answer answer)
    {
        ArgumentNullException.ThrowIfNull(answer);
        _answers.Add(answer);
    }

    public void ReplaceAnswers(IReadOnlyList<Answer> newAnswers)
    {
        _answers.Clear();
        foreach (var a in newAnswers)
            _answers.Add(a);
    }

    public void UpdateText(string text)
    {
        AddTranslation(LanguageCode.Ro, text, null);
    }

    public QuestionTranslation? GetTranslation(LanguageCode languageCode)
        => _translations.FirstOrDefault(x => x.LanguageCode == languageCode);
}
