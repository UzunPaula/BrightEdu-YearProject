using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Infrastructure.Repositories;

// Clasa asta este un repository fals. Ea îmi oferă date ca și cum ar veni dintr-o bază de date, doar că le ține în memorie.
// SRP: acces la date (in-memory).
// ISP: implementează separat citirea și scrierea.
public sealed class InMemoryCourseRepository : ICourseReadRepository, ICourseWriteRepository
{
    private static readonly List<Course> Courses = new()
    {
        new(Guid.NewGuid(), "Pre-Algebra", "Bază pentru ecuații, fracții, proporții."),
        new(Guid.NewGuid(), "Logică", "Raționament, propoziții, reguli simple.")
    };

    public Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken ct = default)
    {
        IReadOnlyList<Course> result = Courses.ToList().AsReadOnly();
        return Task.FromResult(result);
    }

    // Adaugă un curs nou în lista in-memory.
    public Task AddAsync(Course course, CancellationToken ct = default)
    {
        Courses.Add(course);
        return Task.CompletedTask;
    }
}