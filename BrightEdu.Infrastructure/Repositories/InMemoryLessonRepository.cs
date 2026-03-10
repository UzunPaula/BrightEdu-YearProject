using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Infrastructure.Repositories;

// SRP: acces la date pentru lecții (in-memory) și un sample pentru testare.
// Stochează lecțiile într-o listă statică pentru testare.
public sealed class InMemoryLessonRepository : ILessonRepository
{
    // Listă statică care rămâne în memorie cât timp rulează aplicația.
    // Are o lecție sample ca să am date imediat.
    private static readonly List<Lesson> _lessons = new List<Lesson>
    {
        CreateSample()
    };

    // Caută lecția după Id.
    // Returnează Task ca să păstrez aceeași semnătură ca un repo real (DB).
    public Task<Lesson?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        // FirstOrDefault întoarce null dacă nu găsește lecția.
        var lesson = _lessons.FirstOrDefault(l => l.Id == id);
        return Task.FromResult<Lesson?>(lesson);
    }

    // Adaugă o lecție nouă în listă.
    // Task.CompletedTask, fiindcă operația e instanta și nu așteaptă IO.
    public Task AddAsync(Lesson lesson, CancellationToken ct = default)
    {
        _lessons.Add(lesson);
        return Task.CompletedTask;
    }

    // Creează o lecție de exemplu pentru demo și test.
    // Aici modelez un caz real: o lecție cu pași (content + question).
    private static Lesson CreateSample()
    {
        var lessonId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var lesson = new Lesson(lessonId, "Fracții. Introducere");

        lesson.AddStep(new ContentStep(Guid.NewGuid(), 1, "O fracție reprezintă o parte dintr-un întreg."));
        lesson.AddStep(new QuestionStep(
            Guid.NewGuid(),
            2,
            "Care este fracția pentru 1 din 4 părți egale?",
            new List<string> { "1/2", "1/3", "1/4", "2/4" },
            2));

        // Returnez lecția completă.
        return lesson;
    }
}