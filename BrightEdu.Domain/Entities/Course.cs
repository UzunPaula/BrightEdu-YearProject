using BrightEdu.Domain.Enums;
using System.Text;
using System.Text.RegularExpressions;

namespace BrightEdu.Domain.Entities;

public class Course
{
    private readonly List<Lesson> _lessons = new();
    private readonly List<Module> _modules = new();
    private readonly List<CourseTranslation> _translations = new();
    private readonly List<CourseCategory> _courseCategories = new();
    private readonly List<Enrollment> _enrollments = new();

    private Course()
    {
    }

    public Course(Guid id, string title, string? description)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));

        Id = id;
        Slug = BuildSlug(title);
        Level = "Beginner";
        State = CourseState.Draft;
        IsPublished = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        SetDefaultTranslation(title, description);
    }

    public Guid Id { get; private set; }
    public string Slug { get; private set; } = null!;
    public string Level { get; private set; } = null!;
    public CourseState State { get; private set; }
    public bool IsPublished { get; private set; }
    public Guid? ThumbnailMediaAssetId { get; private set; }
    public MediaAsset? ThumbnailMediaAsset { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();
    public IReadOnlyCollection<Module> Modules => _modules.AsReadOnly();
    public IReadOnlyCollection<CourseTranslation> Translations => _translations.AsReadOnly();
    public IReadOnlyCollection<CourseCategory> CourseCategories => _courseCategories.AsReadOnly();
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    public string Title => GetTranslation(LanguageCode.Ro)?.Title ?? _translations.FirstOrDefault()?.Title ?? Slug;
    public string? Description => GetTranslation(LanguageCode.Ro)?.ShortDescription ?? _translations.FirstOrDefault()?.ShortDescription;

    public void SetSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Slug invalid.", nameof(slug));

        Slug = slug.Trim().ToLowerInvariant();
        Touch();
    }

    public void SetLevel(string level)
    {
        if (string.IsNullOrWhiteSpace(level))
            throw new ArgumentException("Nivel invalid.", nameof(level));

        Level = level.Trim();
        Touch();
    }

    public void SetState(CourseState state)
    {
        State = state;
        IsPublished = state == CourseState.Published;
        Touch();
    }

    public void SetThumbnail(Guid? mediaAssetId)
    {
        ThumbnailMediaAssetId = mediaAssetId;
        Touch();
    }

    public void AddModule(Module module)
    {
        ArgumentNullException.ThrowIfNull(module);
        _modules.Add(module);
        Touch();
    }

    public void AddLesson(Lesson lesson)
    {
        ArgumentNullException.ThrowIfNull(lesson);
        _lessons.Add(lesson);
        Touch();
    }

    public void AddTranslation(LanguageCode languageCode, string title, string? shortDescription, string? fullDescription)
    {
        var existing = GetTranslation(languageCode);
        if (existing is null)
        {
            _translations.Add(new CourseTranslation(Guid.NewGuid(), Id, languageCode, title, shortDescription, fullDescription));
        }
        else
        {
            existing.Update(title, shortDescription, fullDescription);
        }

        Touch();
    }

    public CourseTranslation? GetTranslation(LanguageCode languageCode)
        => _translations.FirstOrDefault(x => x.LanguageCode == languageCode);

    private void SetDefaultTranslation(string title, string? description)
    {
        AddTranslation(LanguageCode.Ro, title, description, description);
    }

    private void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    private static string BuildSlug(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Titlul nu poate fi gol.", nameof(title));

        var normalized = title.Trim().ToLowerInvariant();
        normalized = normalized.Replace("c#", "csharp");
        normalized = normalized.Replace(".net", "dotnet");

        var builder = new StringBuilder(normalized.Length);
        foreach (var character in normalized)
        {
            builder.Append(char.IsLetterOrDigit(character) ? character : '-');
        }

        var slug = Regex.Replace(builder.ToString(), "-{2,}", "-").Trim('-');
        return string.IsNullOrWhiteSpace(slug) ? "course" : slug;
    }
}
