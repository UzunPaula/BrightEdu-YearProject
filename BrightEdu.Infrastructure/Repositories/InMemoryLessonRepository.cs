using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Infrastructure.Repositories;

// Repository in-memory pentru lecții.
// Îl folosim temporar pentru testare, până conectăm baza de date prin EF Core.
public sealed class InMemoryLessonRepository : ILessonRepository
{
    // Listă statică de lecții păstrate în memorie pe durata rulării aplicației.
    private static readonly List<Lesson> _lessons = new()
    {
        CreateSample()
    };

    // Returnează lecția găsită după Id sau null dacă nu există.
    public Task<Lesson?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var lesson = _lessons.FirstOrDefault(l => l.Id == id);
        return Task.FromResult(lesson);
    }

    // Adaugă o lecție nouă în lista in-memory.
    public Task AddAsync(Lesson lesson, CancellationToken ct = default)
    {
        _lessons.Add(lesson);
        return Task.CompletedTask;
    }

    // Creează o lecție simplă de exemplu, conform noului model.
    private static Lesson CreateSample()
    {
        var lessonId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var courseId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        return new Lesson(
            lessonId,
            "Fracții. Introducere",
            "O fracție reprezintă o parte dintr-un întreg.",
            1,
            courseId);
    }
}