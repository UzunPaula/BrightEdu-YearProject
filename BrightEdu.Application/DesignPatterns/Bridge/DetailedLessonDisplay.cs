namespace BrightEdu.Application.DesignPatterns.Bridge;

// ConcreteImplementationB
public sealed class DetailedLessonDisplay : ILessonDisplay
{
    public string ShowLesson(string title, string content, int order)
    {
        return $"Lecția: {title}, Ordine: {order}, Conținut: {content}";
    }

    public string ShowLessonWithQuiz(string title, string content, int order, string quizTitle, int questionCount)
    {
        return $"Lecția: {title}, Ordine: {order}, Conținut: {content}, Quiz: {quizTitle}, Număr întrebări: {questionCount}";
    }
}