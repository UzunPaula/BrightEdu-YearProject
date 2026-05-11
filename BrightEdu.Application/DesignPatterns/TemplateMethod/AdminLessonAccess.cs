using BrightEdu.Application.DTOs;
using BrightEdu.Application.Features.Lessons;

namespace BrightEdu.Application.DesignPatterns.TemplateMethod;

/// <summary>
/// Template Method concret — acces administrativ la lecție.
/// Nu verifică înscrierea și nu înregistrează progres.
/// </summary>
public sealed class AdminLessonAccess : LessonAccessTemplate
{
    private readonly IPublicLessonService _lessonService;

    public AdminLessonAccess(IPublicLessonService lessonService)
    {
        _lessonService = lessonService;
    }

    // Adminul are întotdeauna acces la orice lecție
    protected override Task<bool> CanAccessAsync(Guid userId, Guid lessonId, CancellationToken ct)
        => Task.FromResult(true);

    protected override Task<LessonDetailsDto?> LoadLessonAsync(Guid lessonId, CancellationToken ct)
        => _lessonService.GetByIdAsync(lessonId, "ro", ct);
}
