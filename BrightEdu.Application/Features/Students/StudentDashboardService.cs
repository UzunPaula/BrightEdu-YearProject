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
    private readonly IQuizAttemptRepository _quizAttemptRepository;

    public StudentDashboardService(
        IEnrollmentRepository enrollmentRepository,
        ILessonProgressRepository lessonProgressRepository,
        IQuizAttemptRepository quizAttemptRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _lessonProgressRepository = lessonProgressRepository;
        _quizAttemptRepository = quizAttemptRepository;
    }

    public async Task<StudentDashboardDto> GetAsync(Guid studentId, CancellationToken ct = default)
    {
        var enrollments = await _enrollmentRepository.GetForStudentAsync(studentId, ct);
        var progresses = await _lessonProgressRepository.GetForStudentAsync(studentId, ct);
        var quizAttempts = await _quizAttemptRepository.GetAllSubmittedForStudentAsync(studentId, ct);

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
                progress.Lesson.Course?.Title,
                progress.IsCompleted,
                progress.CompletedAt,
                progress.LastOpenedAt))
            .ToList()
            .AsReadOnly();

        var submittedAttempts = quizAttempts.Where(a => a.Score.HasValue).ToList();
        var quizStats = new QuizStatsDto(
            TotalAttempts: quizAttempts.Count,
            TotalPassed: quizAttempts.Count(a => a.Passed),
            UniqueQuizzes: quizAttempts.Select(a => a.QuizId).Distinct().Count(),
            AverageScore: submittedAttempts.Count > 0
                ? Math.Round(submittedAttempts.Average(a => a.Score!.Value), 1)
                : 0m,
            BestScore: submittedAttempts.Count > 0
                ? submittedAttempts.Max(a => a.Score!.Value)
                : 0m);

        return new StudentDashboardDto(courseProgress, recentLessons, quizStats);
    }
}
