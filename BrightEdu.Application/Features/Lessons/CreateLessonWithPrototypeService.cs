using BrightEdu.Application.Interfaces;

namespace BrightEdu.Application.Features.Lessons;

public class CreateLessonWithPrototypeService : ICreateLessonWithPrototypeService
{
    private readonly ILessonRepository _lessonRepository;

    public CreateLessonWithPrototypeService(ILessonRepository lessonRepository)
    {
        _lessonRepository = lessonRepository;
    }

    // Creează o lecție nouă prin clonarea unei lecții existente
    public async Task<Guid> CreateFromPrototypeAsync(Guid sourceLessonId, CancellationToken ct)
    {
        // Caută lecția sursă în repository
        var sourceLesson = await _lessonRepository.GetByIdAsync(sourceLessonId, ct);

        if (sourceLesson is null)
            throw new ArgumentException("Lecția sursă nu a fost găsită.");

        // Clonează lecția folosind pattern-ul Prototype
        var clonedLesson = sourceLesson.Clone();

        // Salvează copia în repository
        await _lessonRepository.AddAsync(clonedLesson, ct);

        return clonedLesson.Id;  // Returnează ID-ul noii lecții
    }
}