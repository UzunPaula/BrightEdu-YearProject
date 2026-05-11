using BrightEdu.Domain.Enums;

namespace BrightEdu.Domain.Entities;

public class Enrollment
{
    private Enrollment()
    {
    }

    public Enrollment(Guid id, Guid studentId, Guid courseId, EnrollmentStatus status)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        if (studentId == Guid.Empty) throw new ArgumentException("StudentId invalid.", nameof(studentId));
        if (courseId == Guid.Empty) throw new ArgumentException("CourseId invalid.", nameof(courseId));

        Id = id;
        StudentId = studentId;
        CourseId = courseId;
        Status = status;
        EnrolledAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid StudentId { get; private set; }
    public User Student { get; private set; } = null!;
    public Guid CourseId { get; private set; }
    public Course Course { get; private set; } = null!;
    public EnrollmentStatus Status { get; private set; }
    public DateTime EnrolledAt { get; private set; }
}
