using BrightEdu.Domain.Enums;

namespace BrightEdu.Domain.Entities;

public class Module
{
    private readonly List<ModuleTranslation> _translations = new();
    private readonly List<Lesson> _lessons = new();

    private Module()
    {
    }

    public Module(Guid id, Guid courseId, int order, string title, string? description)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        if (courseId == Guid.Empty) throw new ArgumentException("CourseId invalid.", nameof(courseId));
        if (order <= 0) throw new ArgumentOutOfRangeException(nameof(order));

        Id = id;
        CourseId = courseId;
        Order = order;
        State = CourseState.Draft;
        AddTranslation(LanguageCode.Ro, title, description);
    }

    public Guid Id { get; private set; }
    public Guid CourseId { get; private set; }
    public Course Course { get; private set; } = null!;
    public int Order { get; private set; }
    public CourseState State { get; private set; }
    public Quiz? Quiz { get; private set; }

    public IReadOnlyCollection<ModuleTranslation> Translations => _translations.AsReadOnly();
    public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();

    public string Title => GetTranslation(LanguageCode.Ro)?.Title ?? _translations.FirstOrDefault()?.Title ?? string.Empty;

    public void SetState(CourseState state) => State = state;

    public void AddTranslation(LanguageCode languageCode, string title, string? description)
    {
        var existing = GetTranslation(languageCode);
        if (existing is null)
        {
            _translations.Add(new ModuleTranslation(Guid.NewGuid(), Id, languageCode, title, description));
        }
        else
        {
            existing.Update(title, description);
        }
    }

    public void AddLesson(Lesson lesson)
    {
        ArgumentNullException.ThrowIfNull(lesson);
        _lessons.Add(lesson);
    }

    public ModuleTranslation? GetTranslation(LanguageCode languageCode)
        => _translations.FirstOrDefault(x => x.LanguageCode == languageCode);
}
