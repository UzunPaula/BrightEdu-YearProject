using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Infrastructure.Repositories;

// SRP: acces la date pentru lecții (in-memory) și un sample pentru testare.
public sealed class InMemoryLessonRepository : ILessonRepository
{
    private static readonly Lesson SampleLesson = CreateSample();

    public Task<Lesson?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        if (id == SampleLesson.Id) return Task.FromResult<Lesson?>(SampleLesson);
        return Task.FromResult<Lesson?>(null);
    }

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

        return lesson;
    }
}