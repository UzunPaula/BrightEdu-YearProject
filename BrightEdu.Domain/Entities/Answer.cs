using BrightEdu.Domain.Enums;

namespace BrightEdu.Domain.Entities;

public class Answer
{
    private readonly List<AnswerOptionTranslation> _translations = new();

    private Answer()
    {
    }

    public Answer(Guid id, string text, bool isCorrect, Guid questionId)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        if (questionId == Guid.Empty) throw new ArgumentException("QuestionId invalid.", nameof(questionId));

        Id = id;
        IsCorrect = isCorrect;
        QuestionId = questionId;
        Order = 1;
        AddTranslation(LanguageCode.Ro, text);
    }

    public Guid Id { get; private set; }
    public Guid QuestionId { get; private set; }
    public Question Question { get; private set; } = null!;
    public bool IsCorrect { get; private set; }
    public int Order { get; private set; }

    public IReadOnlyCollection<AnswerOptionTranslation> Translations => _translations.AsReadOnly();

    public string Text => GetTranslation(LanguageCode.Ro)?.Text ?? _translations.FirstOrDefault()?.Text ?? string.Empty;

    public void Configure(bool isCorrect, int order)
    {
        if (order <= 0) throw new ArgumentOutOfRangeException(nameof(order));

        IsCorrect = isCorrect;
        Order = order;
    }

    public void AddTranslation(LanguageCode languageCode, string text)
    {
        var existing = GetTranslation(languageCode);
        if (existing is null)
        {
            _translations.Add(new AnswerOptionTranslation(Guid.NewGuid(), Id, languageCode, text));
        }
        else
        {
            existing.Update(text);
        }
    }

    public AnswerOptionTranslation? GetTranslation(LanguageCode languageCode)
        => _translations.FirstOrDefault(x => x.LanguageCode == languageCode);
}
