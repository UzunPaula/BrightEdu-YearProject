using BrightEdu.Domain.Enums;

namespace BrightEdu.Domain.Entities;

public class QuestionTranslation
{
    private QuestionTranslation()
    {
    }

    public QuestionTranslation(Guid id, Guid questionId, LanguageCode languageCode, string text, string? explanation)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        if (questionId == Guid.Empty) throw new ArgumentException("QuestionId invalid.", nameof(questionId));
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("Text invalid.", nameof(text));

        Id = id;
        QuestionId = questionId;
        LanguageCode = languageCode;
        Text = text.Trim();
        Explanation = string.IsNullOrWhiteSpace(explanation) ? null : explanation.Trim();
    }

    public Guid Id { get; private set; }
    public Guid QuestionId { get; private set; }
    public Question Question { get; private set; } = null!;
    public LanguageCode LanguageCode { get; private set; }
    public string Text { get; private set; } = null!;
    public string? Explanation { get; private set; }

    public void Update(string text, string? explanation)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text invalid.", nameof(text));

        Text = text.Trim();
        Explanation = string.IsNullOrWhiteSpace(explanation) ? null : explanation.Trim();
    }
}
