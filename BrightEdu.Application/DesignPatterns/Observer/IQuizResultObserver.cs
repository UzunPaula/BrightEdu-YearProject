namespace BrightEdu.Application.DesignPatterns.Observer;

/// <summary>
/// Observer — este notificat când un student trimite un quiz.
/// </summary>
public interface IQuizResultObserver
{
    Task OnQuizSubmittedAsync(Guid studentId, Guid? lessonId, bool passed, CancellationToken ct = default);
}
