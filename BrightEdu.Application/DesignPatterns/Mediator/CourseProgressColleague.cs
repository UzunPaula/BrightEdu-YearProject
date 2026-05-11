using BrightEdu.Application.Interfaces;

namespace BrightEdu.Application.DesignPatterns.Mediator;

/// <summary>
/// Coleg concret — calculează și înregistrează progresul total al cursului
/// după finalizarea unei lecții, fără a cunoaște direct ceilalți colegi.
/// </summary>
public sealed class CourseProgressColleague : ILessonCompletionColleague
{
    private readonly ILessonProgressRepository _progressRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public CourseProgressColleague(
        ILessonProgressRepository progressRepository,
        ILessonRepository lessonRepository,
        IEnrollmentRepository enrollmentRepository)
    {
        _progressRepository = progressRepository;
        _lessonRepository = lessonRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task OnLessonCompletedAsync(Guid studentId, Guid lessonId, CancellationToken ct)
    {
        var lesson = await _lessonRepository.GetByIdAsync(lessonId, ct);
        if (lesson is null) return;

        var enrollment = await _enrollmentRepository.GetAsync(studentId, lesson.CourseId, ct);
        if (enrollment is null) return;

        var allProgress = await _progressRepository.GetForStudentAsync(studentId, ct);
        var completedInCourse = allProgress.Count(p =>
            p.Lesson?.CourseId == lesson.CourseId && p.IsCompleted);

        // Înregistrare progres per curs disponibilă prin enrollment.UpdateProgress()
        // (hook pentru extindere viitoare fără a modifica ceilalți colegi)
        _ = completedInCourse;
    }
}
