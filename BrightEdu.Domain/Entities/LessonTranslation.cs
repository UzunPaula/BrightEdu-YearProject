using BrightEdu.Domain.Enums;

namespace BrightEdu.Domain.Entities;

public class LessonTranslation
{
    private LessonTranslation()
    {
    }

    public LessonTranslation(Guid id, Guid lessonId, LanguageCode languageCode, string title, string summary)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        if (lessonId == Guid.Empty) throw new ArgumentException("LessonId invalid.", nameof(lessonId));
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title invalid.", nameof(title));
        if (string.IsNullOrWhiteSpace(summary)) throw new ArgumentException("Summary invalid.", nameof(summary));

        Id = id;
        LessonId = lessonId;
        LanguageCode = languageCode;
        Title = title.Trim();
        Summary = summary.Trim();
    }

    public Guid Id { get; private set; }
    public Guid LessonId { get; private set; }
    public Lesson Lesson { get; private set; } = null!;
    public LanguageCode LanguageCode { get; private set; }
    public string Title { get; private set; } = null!;
    public string Summary { get; private set; } = null!;

    public void Update(string title, string summary)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title invalid.", nameof(title));
        if (string.IsNullOrWhiteSpace(summary)) throw new ArgumentException("Summary invalid.", nameof(summary));

        Title = title.Trim();
        Summary = summary.Trim();
    }
}
