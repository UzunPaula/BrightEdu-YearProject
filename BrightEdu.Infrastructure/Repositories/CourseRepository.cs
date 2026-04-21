using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BrightEdu.Infrastructure.Repositories;

// Repository real pentru Course, bazat pe EF Core.
// Acesta înlocuiește varianta in-memory atunci când vrem să lucrăm cu baza de date reală.
public sealed class CourseRepository : ICourseReadRepository, ICourseWriteRepository
{
    private readonly AppDbContext _dbContext;

    public CourseRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Returnează toate cursurile din baza de date.
    public async Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken ct = default)
    {
        return await _dbContext.Courses
            .AsNoTracking()
            .ToListAsync(ct);
    }

    // Adaugă un curs nou în baza de date.
    public async Task AddAsync(Course course, CancellationToken ct = default)
    {
        await _dbContext.Courses.AddAsync(course, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}