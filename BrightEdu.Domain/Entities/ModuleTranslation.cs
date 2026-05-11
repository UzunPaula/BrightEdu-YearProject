using BrightEdu.Domain.Enums;

namespace BrightEdu.Domain.Entities;

public class ModuleTranslation
{
    private ModuleTranslation()
    {
    }

    public ModuleTranslation(Guid id, Guid moduleId, LanguageCode languageCode, string title, string? description)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        if (moduleId == Guid.Empty) throw new ArgumentException("ModuleId invalid.", nameof(moduleId));
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title invalid.", nameof(title));

        Id = id;
        ModuleId = moduleId;
        LanguageCode = languageCode;
        Title = title.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    public Guid Id { get; private set; }
    public Guid ModuleId { get; private set; }
    public Module Module { get; private set; } = null!;
    public LanguageCode LanguageCode { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }

    public void Update(string title, string? description)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title invalid.", nameof(title));

        Title = title.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}
