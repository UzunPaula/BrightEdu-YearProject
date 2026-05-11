namespace BrightEdu.Application.DesignPatterns.Mediator;

/// <summary>
/// Mediator — coordonează comunicarea între componente fără coupling direct.
/// Notifică toate componentele interesate când o lecție este finalizată.
/// </summary>
public interface ILessonCompletionMediator
{
    Task NotifyLessonCompletedAsync(Guid studentId, Guid lessonId, CancellationToken ct = default);
    void Register(ILessonCompletionColleague colleague);
}
