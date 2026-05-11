using BrightEdu.Application.DTOs;

namespace BrightEdu.Application.DesignPatterns.TemplateMethod;

/// <summary>
/// Template Method — definește scheletul fluxului de acces la o lecție.
/// Subclasele implementează pașii specifici (verificare acces, tracking progres).
/// </summary>
public abstract class LessonAccessTemplate
{
    // Metoda șablon — nu poate fi suprascrisă
    public async Task<LessonDetailsDto?> AccessAsync(Guid userId, Guid lessonId, CancellationToken ct = default)
    {
        if (!await CanAccessAsync(userId, lessonId, ct))
            return null;

        await OnBeforeAccessAsync(userId, lessonId, ct);
        var lesson = await LoadLessonAsync(lessonId, ct);
        if (lesson is not null)
            await OnAfterAccessAsync(userId, lessonId, ct);

        return lesson;
    }

    // Pași obligatorii
    protected abstract Task<bool> CanAccessAsync(Guid userId, Guid lessonId, CancellationToken ct);
    protected abstract Task<LessonDetailsDto?> LoadLessonAsync(Guid lessonId, CancellationToken ct);

    // Pași opționali (hook-uri)
    protected virtual Task OnBeforeAccessAsync(Guid userId, Guid lessonId, CancellationToken ct) => Task.CompletedTask;
    protected virtual Task OnAfterAccessAsync(Guid userId, Guid lessonId, CancellationToken ct) => Task.CompletedTask;
}
