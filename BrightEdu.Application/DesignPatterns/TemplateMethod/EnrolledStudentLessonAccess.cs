using BrightEdu.Application.DTOs;
using BrightEdu.Application.Features.Lessons;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.DesignPatterns.TemplateMethod;

/// <summary>
/// Template Method concret — acces pentru student înscris.
/// Verifică înscrierea la curs și înregistrează deschiderea lecției.
/// </summary>
public sealed class EnrolledStudentLessonAccess : LessonAccessTemplate
{
    private readonly IPublicLessonService _lessonService;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ILessonProgressRepository _progressRepository;
    private readonly ILessonRepository _lessonRepository;

    public EnrolledStudentLessonAccess(
        IPublicLessonService lessonService,
        IEnrollmentRepository enrollmentRepository,
        ILessonProgressRepository progressRepository,
        ILessonRepository lessonRepository)
    {
        _lessonService = lessonService;
        _enrollmentRepository = enrollmentRepository;
        _progressRepository = progressRepository;
        _lessonRepository = lessonRepository;
    }

    protected override async Task<bool> CanAccessAsync(Guid userId, Guid lessonId, CancellationToken ct)
    {
        var lesson = await _lessonRepository.GetByIdAsync(lessonId, ct);
        if (lesson is null) return false;

        var enrollment = await _enrollmentRepository.GetAsync(userId, lesson.CourseId, ct);
        return enrollment is not null;
    }

    protected override Task<LessonDetailsDto?> LoadLessonAsync(Guid lessonId, CancellationToken ct)
        => _lessonService.GetByIdAsync(lessonId, "ro", ct);

    // Hook — înregistrează deschiderea lecției pentru tracking progres
    protected override async Task OnAfterAccessAsync(Guid userId, Guid lessonId, CancellationToken ct)
    {
        var progress = await _progressRepository.GetAsync(userId, lessonId, ct);
        if (progress is null)
        {
            progress = new LessonProgress(Guid.NewGuid(), userId, lessonId);
            await _progressRepository.AddAsync(progress, ct);
        }

        progress.MarkOpened();
        await _progressRepository.SaveChangesAsync(ct);
    }
}
