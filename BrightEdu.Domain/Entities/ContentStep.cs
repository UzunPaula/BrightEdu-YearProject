namespace BrightEdu.Domain.Entities;

// moștenire: ContentStep este un LessonStep
public sealed class ContentStep : LessonStep
{
    public string Content { get; private set; }
    public ContentStep(Guid id, int order, string content) : base(id, order)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content nu poate fi gol.", nameof(content));

        Content = content.Trim();
    }
}