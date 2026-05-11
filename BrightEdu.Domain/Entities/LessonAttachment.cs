namespace BrightEdu.Domain.Entities;

public class LessonAttachment
{
    private LessonAttachment()
    {
    }

    public LessonAttachment(Guid id, Guid lessonId, Guid mediaAssetId, string displayName)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        if (lessonId == Guid.Empty) throw new ArgumentException("LessonId invalid.", nameof(lessonId));
        if (mediaAssetId == Guid.Empty) throw new ArgumentException("MediaAssetId invalid.", nameof(mediaAssetId));
        if (string.IsNullOrWhiteSpace(displayName)) throw new ArgumentException("DisplayName invalid.", nameof(displayName));

        Id = id;
        LessonId = lessonId;
        MediaAssetId = mediaAssetId;
        DisplayName = displayName.Trim();
    }

    public LessonAttachment(Guid id, Guid lessonId, string externalUrl, string displayName)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        if (lessonId == Guid.Empty) throw new ArgumentException("LessonId invalid.", nameof(lessonId));
        if (string.IsNullOrWhiteSpace(externalUrl)) throw new ArgumentException("ExternalUrl invalid.", nameof(externalUrl));
        if (string.IsNullOrWhiteSpace(displayName)) throw new ArgumentException("DisplayName invalid.", nameof(displayName));

        Id = id;
        LessonId = lessonId;
        ExternalUrl = externalUrl.Trim();
        DisplayName = displayName.Trim();
    }

    public Guid Id { get; private set; }
    public Guid LessonId { get; private set; }
    public Lesson Lesson { get; private set; } = null!;
    public Guid? MediaAssetId { get; private set; }
    public MediaAsset? MediaAsset { get; private set; }
    public string? ExternalUrl { get; private set; }
    public string DisplayName { get; private set; } = null!;
}
