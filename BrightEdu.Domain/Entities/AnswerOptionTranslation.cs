using BrightEdu.Domain.Enums;

namespace BrightEdu.Domain.Entities;

public class AnswerOptionTranslation
{
    private AnswerOptionTranslation()
    {
    }

    public AnswerOptionTranslation(Guid id, Guid answerId, LanguageCode languageCode, string text)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        if (answerId == Guid.Empty) throw new ArgumentException("AnswerId invalid.", nameof(answerId));
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("Text invalid.", nameof(text));

        Id = id;
        AnswerId = answerId;
        LanguageCode = languageCode;
        Text = text.Trim();
    }

    public Guid Id { get; private set; }
    public Guid AnswerId { get; private set; }
    public Answer Answer { get; private set; } = null!;
    public LanguageCode LanguageCode { get; private set; }
    public string Text { get; private set; } = null!;

    public void Update(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text invalid.", nameof(text));

        Text = text.Trim();
    }
}
