namespace BrightEdu.Application.DesignPatterns.Mediator;

/// <summary>
/// Coleg — componentă care reacționează la evenimentele mediate.
/// </summary>
public interface ILessonCompletionColleague
{
    Task OnLessonCompletedAsync(Guid studentId, Guid lessonId, CancellationToken ct);
}
