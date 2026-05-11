namespace BrightEdu.Domain.Entities;

public class LessonProgress
{
    private LessonProgress()
    {
    }

    public LessonProgress(Guid id, Guid studentId, Guid lessonId)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        if (studentId == Guid.Empty) throw new ArgumentException("StudentId invalid.", nameof(studentId));
        if (lessonId == Guid.Empty) throw new ArgumentException("LessonId invalid.", nameof(lessonId));

        Id = id;
        StudentId = studentId;
        LessonId = lessonId;
        LastOpenedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid StudentId { get; private set; }
    public User Student { get; private set; } = null!;
    public Guid LessonId { get; private set; }
    public Lesson Lesson { get; private set; } = null!;
    public bool IsCompleted { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime LastOpenedAt { get; private set; }

    public void MarkOpened()
    {
        LastOpenedAt = DateTime.UtcNow;
    }

    public void MarkCompleted(bool isCompleted)
    {
        IsCompleted = isCompleted;
        CompletedAt = isCompleted ? DateTime.UtcNow : null;
        LastOpenedAt = DateTime.UtcNow;
    }
}
