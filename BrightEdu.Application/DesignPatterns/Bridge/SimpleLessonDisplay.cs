namespace BrightEdu.Application.DesignPatterns.Bridge;

// ConcreteImplementationA
public sealed class SimpleLessonDisplay : ILessonDisplay
{
    public string ShowLesson(string title, string content, int order)
    {
        return $"Lecția: {title}";
    }

    public string ShowLessonWithQuiz(string title, string content, int order, string quizTitle, int questionCount)
    {
        return $"Lecția: {title}, Quiz: {quizTitle}";
    }
}