using BrightEdu.Domain.Enums;

namespace BrightEdu.Domain.Entities;

public class LessonContentBlock
{
    private LessonContentBlock()
    {
    }

    public LessonContentBlock(Guid id, Guid lessonId, int order, ContentBlockType blockType, string configJson)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        if (lessonId == Guid.Empty) throw new ArgumentException("LessonId invalid.", nameof(lessonId));
        if (order <= 0) throw new ArgumentOutOfRangeException(nameof(order));
        if (string.IsNullOrWhiteSpace(configJson)) throw new ArgumentException("Config invalid.", nameof(configJson));

        Id = id;
        LessonId = lessonId;
        Order = order;
        BlockType = blockType;
        ConfigJson = configJson.Trim();
    }

    public Guid Id { get; private set; }
    public Guid LessonId { get; private set; }
    public Lesson Lesson { get; private set; } = null!;
    public int Order { get; private set; }
    public ContentBlockType BlockType { get; private set; }
    public string ConfigJson { get; private set; } = null!;
}
