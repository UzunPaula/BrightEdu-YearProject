namespace BrightEdu.Domain.Entities;

// moștenire: ContentStep este un LessonStep
public class ContentStep : LessonStep
{
    public override string Type => "content";
    public string Content { get; private set; }
    public ContentStep(Guid id, int order, string content) : base(id, order)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content nu poate fi gol.", nameof(content));

        Content = content.Trim();
    }
    
    public override LessonStep Clone()
    {
        return new ContentStep(
            Guid.NewGuid(), // ID NOU (nu același cu originalul)
            Order,
            Content); // Același conținut, dar în obiect NOU
    }
}
