using BrightEdu.Domain.Enums;

namespace BrightEdu.Domain.Entities;

public class Lesson
{
    private readonly List<LessonTranslation> _translations = new();
    private readonly List<LessonContentBlock> _contentBlocks = new();
    private readonly List<LessonAttachment> _attachments = new();
    private readonly List<LessonProgress> _progressEntries = new();

    private Lesson()
    {
    }

    public Lesson(Guid id, string title, string content, int order, Guid courseId)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        if (courseId == Guid.Empty) throw new ArgumentException("CourseId invalid.", nameof(courseId));
        if (order <= 0) throw new ArgumentException("Order trebuie sa fie mai mare ca 0.", nameof(order));

        Id = id;
        CourseId = courseId;
        Order = order;
        State = LessonState.Draft;
        EstimatedMinutes = 10;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        SetDefaultTranslation(title, content);
        AddContentBlock(ContentBlockType.Text, 1, $"{{\"content\":\"{EscapeJson(content)}\"}}");
    }

    public Guid Id { get; private set; }
    public Guid CourseId { get; private set; }
    public Course Course { get; private set; } = null!;
    public Guid? ModuleId { get; private set; }
    public Module? Module { get; private set; }
    public int Order { get; private set; }
    public LessonState State { get; private set; }
    public int EstimatedMinutes { get; private set; }
    public bool CodeEditorEnabled { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public Quiz? Quiz { get; private set; }

    public IReadOnlyCollection<LessonTranslation> Translations => _translations.AsReadOnly();
    public IReadOnlyCollection<LessonContentBlock> ContentBlocks => _contentBlocks.AsReadOnly();
    public IReadOnlyCollection<LessonAttachment> Attachments => _attachments.AsReadOnly();
    public IReadOnlyCollection<LessonProgress> ProgressEntries => _progressEntries.AsReadOnly();

    public string Title => GetTranslation(LanguageCode.Ro)?.Title ?? _translations.FirstOrDefault()?.Title ?? string.Empty;
    public string Content => GetPrimaryContentBlockText() ?? GetTranslation(LanguageCode.Ro)?.Summary ?? string.Empty;

    public void AssignModule(Guid? moduleId)
    {
        ModuleId = moduleId;
        Touch();
    }

    public void SetOrder(int order)
    {
        if (order <= 0)
            throw new ArgumentException("Order trebuie sa fie mai mare ca 0.", nameof(order));

        Order = order;
        Touch();
    }

    public void SetEstimatedMinutes(int estimatedMinutes)
    {
        if (estimatedMinutes <= 0)
            throw new ArgumentException("Durata estimata trebuie sa fie pozitiva.", nameof(estimatedMinutes));

        EstimatedMinutes = estimatedMinutes;
        Touch();
    }

    public void SetState(LessonState state)
    {
        State = state;
        Touch();
    }

    public void EnableCodeEditor(bool enabled)
    {
        CodeEditorEnabled = enabled;
        Touch();
    }

    public void AddTranslation(LanguageCode languageCode, string title, string summary)
    {
        var existing = GetTranslation(languageCode);
        if (existing is null)
        {
            _translations.Add(new LessonTranslation(Guid.NewGuid(), Id, languageCode, title, summary));
        }
        else
        {
            existing.Update(title, summary);
        }

        Touch();
    }

    public void AddContentBlock(ContentBlockType blockType, int order, string configJson)
    {
        _contentBlocks.Add(new LessonContentBlock(Guid.NewGuid(), Id, order, blockType, configJson));
        Touch();
    }

    public void AddAttachment(Guid mediaAssetId, string displayName)
    {
        _attachments.Add(new LessonAttachment(Guid.NewGuid(), Id, mediaAssetId, displayName));
        Touch();
    }

    public void ClearContentBlocks()
    {
        _contentBlocks.Clear();
        Touch();
    }

    public void RemoveAttachment(Guid attachmentId)
    {
        var item = _attachments.FirstOrDefault(a => a.Id == attachmentId);
        if (item is not null)
        {
            _attachments.Remove(item);
            Touch();
        }
    }

    public LessonTranslation? GetTranslation(LanguageCode languageCode)
        => _translations.FirstOrDefault(x => x.LanguageCode == languageCode);

    private void SetDefaultTranslation(string title, string content)
    {
        AddTranslation(LanguageCode.Ro, title, content);
    }

    private string? GetPrimaryContentBlockText()
    {
        return _contentBlocks
            .Where(x => x.BlockType == ContentBlockType.Text)
            .OrderBy(x => x.Order)
            .Select(x => x.ConfigJson)
            .FirstOrDefault();
    }

    private void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    private static string EscapeJson(string value)
        => value.Replace("\\", "\\\\").Replace("\"", "\\\"");
}
