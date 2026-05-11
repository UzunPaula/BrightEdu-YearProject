using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;

namespace BrightEdu.Application.Features.Students;

public interface IStudentDashboardService
{
    Task<StudentDashboardDto> GetAsync(Guid studentId, CancellationToken ct = default);
}

public sealed class StudentDashboardService : IStudentDashboardService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ILessonProgressRepository _lessonProgressRepository;

    public StudentDashboardService(
        IEnrollmentRepository enrollmentRepository,
        ILessonProgressRepository lessonProgressRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _lessonProgressRepository = lessonProgressRepository;
    }

    public async Task<StudentDashboardDto> GetAsync(Guid studentId, CancellationToken ct = default)
    {
        var enrollments = await _enrollmentRepository.GetForStudentAsync(studentId, ct);
        var progresses = await _lessonProgressRepository.GetForStudentAsync(studentId, ct);

        var courseProgress = enrollments
            .Select(enrollment =>
            {
                var totalLessons = enrollment.Course.Lessons.Count;
                var completedLessons = progresses.Count(progress =>
                    progress.IsCompleted &&
                    progress.Lesson.CourseId == enrollment.CourseId);
                var completion = totalLessons == 0 ? 0m : Math.Round((decimal)completedLessons / totalLessons * 100m, 2);

                return new StudentCourseProgressDto(
                    enrollment.CourseId,
                    enrollment.Course.Slug,
                    enrollment.Course.Title,
                    totalLessons,
                    completedLessons,
                    completion,
                    enrollment.EnrolledAt);
            })
            .OrderByDescending(x => x.EnrolledAt)
            .ToList()
            .AsReadOnly();

        var recentLessons = progresses
            .OrderByDescending(x => x.LastOpenedAt)
            .Take(10)
            .Select(progress => new LessonProgressDto(
                progress.LessonId,
                progress.Lesson.Title,
                progress.Lesson.Course?.Slug,
                progress.IsCompleted,
                progress.CompletedAt,
                progress.LastOpenedAt))
            .ToList()
            .AsReadOnly();

        return new StudentDashboardDto(courseProgress, recentLessons);
    }
}
