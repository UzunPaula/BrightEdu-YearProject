namespace BrightEdu.Application.DesignPatterns.Mediator;

/// <summary>
/// Mediator concret — notifică toți colegii înregistrați când o lecție este completată.
/// </summary>
public sealed class LessonCompletionMediator : ILessonCompletionMediator
{
    private readonly List<ILessonCompletionColleague> _colleagues = new();

    public void Register(ILessonCompletionColleague colleague)
    {
        if (!_colleagues.Contains(colleague))
            _colleagues.Add(colleague);
    }

    public async Task NotifyLessonCompletedAsync(Guid studentId, Guid lessonId, CancellationToken ct = default)
    {
        foreach (var colleague in _colleagues)
            await colleague.OnLessonCompletedAsync(studentId, lessonId, ct);
    }
}
