using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.DesignPatterns.Bridge;

// RefinedAbstraction
public sealed class LessonWithQuizScreen : LessonScreen
{
    public LessonWithQuizScreen(ILessonDisplay display)
        : base(display)
    {
    }

    public override string Render(Lesson lesson)
    {
        if (lesson.Quiz is null)
            throw new ArgumentException("Lecția nu are quiz.");

        return display.ShowLessonWithQuiz(
            lesson.Title,
            lesson.Content,
            lesson.Order,
            lesson.Quiz.Title,
            lesson.Quiz.Questions.Count);
    }
}