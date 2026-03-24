namespace BrightEdu.Application.DesignPatterns.Composite.Steps;

// Interfață comună pentru orice componentă din structura lecției.
public interface ILessonComponent
{
    string Title { get; }
    void Display(int level = 0);
}