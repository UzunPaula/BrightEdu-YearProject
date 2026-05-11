using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Domain.Enums;

namespace BrightEdu.Application.Features.Students;

public interface ICourseEnrollmentService
{
    Task<EnrollmentResultDto> EnrollAsync(Guid studentId, Guid courseId, CancellationToken ct = default);
    Task<EnrollmentStatusDto> GetStatusAsync(Guid studentId, Guid courseId, CancellationToken ct = default);
    Task<EnrollmentStatusDto> UnenrollAsync(Guid studentId, Guid courseId, CancellationToken ct = default);
}

public sealed class CourseEnrollmentService : ICourseEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICourseCatalogRepository _courseCatalogRepository;

    public CourseEnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        ICourseCatalogRepository courseCatalogRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseCatalogRepository = courseCatalogRepository;
    }

    public async Task<EnrollmentResultDto> EnrollAsync(Guid studentId, Guid courseId, CancellationToken ct = default)
    {
        var existing = await _enrollmentRepository.GetAsync(studentId, courseId, ct);
        if (existing is not null)
        {
            return new EnrollmentResultDto(existing.Id, existing.CourseId, existing.EnrolledAt, existing.Status.ToString());
        }

        var courses = await _courseCatalogRepository.GetPublishedCoursesAsync(ct);
        var course = courses.FirstOrDefault(x => x.Id == courseId);
        if (course is null)
            throw new InvalidOperationException("Cursul nu a fost găsit sau nu este public.");

        var enrollment = new Enrollment(Guid.NewGuid(), studentId, courseId, EnrollmentStatus.Active);
        await _enrollmentRepository.AddAsync(enrollment, ct);

        return new EnrollmentResultDto(enrollment.Id, enrollment.CourseId, enrollment.EnrolledAt, enrollment.Status.ToString());
    }

    public async Task<EnrollmentStatusDto> GetStatusAsync(Guid studentId, Guid courseId, CancellationToken ct = default)
    {
        var enrollment = await _enrollmentRepository.GetAsync(studentId, courseId, ct);
        return enrollment is null
            ? new EnrollmentStatusDto(courseId, false, null, null)
            : new EnrollmentStatusDto(courseId, true, enrollment.EnrolledAt, enrollment.Status.ToString());
    }

    public async Task<EnrollmentStatusDto> UnenrollAsync(Guid studentId, Guid courseId, CancellationToken ct = default)
    {
        var enrollment = await _enrollmentRepository.GetAsync(studentId, courseId, ct);
        if (enrollment is null)
            return new EnrollmentStatusDto(courseId, false, null, null);

        await _enrollmentRepository.RemoveAsync(enrollment, ct);
        return new EnrollmentStatusDto(courseId, false, null, null);
    }
}
