namespace BrightEdu.Application.DesignPatterns.Bridge;

// Implementation
public interface ILessonDisplay
{
    string ShowLesson(string title, string content, int order);
    string ShowLessonWithQuiz(string title, string content, int order, string quizTitle, int questionCount);
}