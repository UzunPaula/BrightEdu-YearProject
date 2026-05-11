using BrightEdu.Domain.Enums;

namespace BrightEdu.Domain.Entities;

public class CourseTranslation
{
    private CourseTranslation()
    {
    }

    public CourseTranslation(Guid id, Guid courseId, LanguageCode languageCode, string title, string? shortDescription, string? fullDescription)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        if (courseId == Guid.Empty) throw new ArgumentException("CourseId invalid.", nameof(courseId));
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title invalid.", nameof(title));

        Id = id;
        CourseId = courseId;
        LanguageCode = languageCode;
        Title = title.Trim();
        ShortDescription = string.IsNullOrWhiteSpace(shortDescription) ? null : shortDescription.Trim();
        FullDescription = string.IsNullOrWhiteSpace(fullDescription) ? null : fullDescription.Trim();
    }

    public Guid Id { get; private set; }
    public Guid CourseId { get; private set; }
    public Course Course { get; private set; } = null!;
    public LanguageCode LanguageCode { get; private set; }
    public string Title { get; private set; } = null!;
    public string? ShortDescription { get; private set; }
    public string? FullDescription { get; private set; }

    public void Update(string title, string? shortDescription, string? fullDescription)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title invalid.", nameof(title));

        Title = title.Trim();
        ShortDescription = string.IsNullOrWhiteSpace(shortDescription) ? null : shortDescription.Trim();
        FullDescription = string.IsNullOrWhiteSpace(fullDescription) ? null : fullDescription.Trim();
    }
}
