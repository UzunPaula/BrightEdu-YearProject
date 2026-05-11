namespace BrightEdu.Application.DTOs;

public sealed record EnrollmentResultDto(
    Guid EnrollmentId,
    Guid CourseId,
    DateTime EnrolledAt,
    string Status);

public sealed record EnrollmentStatusDto(
    Guid CourseId,
    bool IsEnrolled,
    DateTime? EnrolledAt,
    string? Status);

public sealed record UpdateLessonProgressRequestDto(
    Guid LessonId,
    bool IsCompleted);

public sealed record LessonProgressDto(
    Guid LessonId,
    string LessonTitle,
    string? CourseSlug,
    string? CourseTitle,
    bool IsCompleted,
    DateTime? CompletedAt,
    DateTime LastOpenedAt);

public sealed record StudentCourseProgressDto(
    Guid CourseId,
    string Slug,
    string Title,
    int TotalLessons,
    int CompletedLessons,
    decimal CompletionPercentage,
    DateTime EnrolledAt);

public sealed record StudentDashboardDto(
    IReadOnlyList<StudentCourseProgressDto> Courses,
    IReadOnlyList<LessonProgressDto> RecentLessons);
