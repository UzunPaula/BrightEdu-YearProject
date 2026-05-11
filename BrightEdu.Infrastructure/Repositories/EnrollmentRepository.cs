using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BrightEdu.Infrastructure.Repositories;

public sealed class EnrollmentRepository : IEnrollmentRepository
{
    private readonly AppDbContext _dbContext;

    public EnrollmentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Enrollment?> GetAsync(Guid studentId, Guid courseId, CancellationToken ct = default)
    {
        return await _dbContext.Enrollments
            .Include(x => x.Course)
            .FirstOrDefaultAsync(x => x.StudentId == studentId && x.CourseId == courseId, ct);
    }

    public async Task<IReadOnlyList<Enrollment>> GetForStudentAsync(Guid studentId, CancellationToken ct = default)
    {
        return await _dbContext.Enrollments
            .Include(x => x.Course)
            .ThenInclude(x => x.Translations)
            .Include(x => x.Course)
            .ThenInclude(x => x.Lessons)
            .Where(x => x.StudentId == studentId)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task AddAsync(Enrollment enrollment, CancellationToken ct = default)
    {
        await _dbContext.Enrollments.AddAsync(enrollment, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(Enrollment enrollment, CancellationToken ct = default)
    {
        _dbContext.Enrollments.Remove(enrollment);
        await _dbContext.SaveChangesAsync(ct);
    }
}
