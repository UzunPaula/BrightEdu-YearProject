using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.DesignPatterns.Bridge;

// Abstraction
public class LessonScreen
{
    protected readonly ILessonDisplay display;

    public LessonScreen(ILessonDisplay display)
    {
        this.display = display;
    }

    public virtual string Render(Lesson lesson)
    {
        return display.ShowLesson(
            lesson.Title,
            lesson.Content,
            lesson.Order);
    }
}