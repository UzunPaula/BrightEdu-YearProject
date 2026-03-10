using BrightEdu.Application.DesignPatterns.Builder;
using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;

namespace BrightEdu.Application.Features.Lessons;

// Serviciu aplicație care orchestrează crearea unei lecții folosind pattern-ul Builder.
public class CreateLessonWithBuilderService : ICreateLessonWithBuilderService
{
    private readonly ILessonDirector _lessonDirector;
    private readonly ILessonRepository _lessonRepository;

    public CreateLessonWithBuilderService(
        ILessonDirector lessonDirector,
        ILessonRepository lessonRepository)
    {
        _lessonDirector = lessonDirector;
        _lessonRepository = lessonRepository;
    }

    // Creează o lecție nouă folosind directorul și builder-ul, apoi o salvează în repository
    // Datele pentru crearea lecției. Token pentru anulare ct.
    public async Task CreateAsync(CreateLessonRequestDto dto, CancellationToken ct)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        // 1. Construiește entitatea Lesson folosind Director + Builder
        var lesson = _lessonDirector.Construct(dto);
        
        // 2. Persistă entitatea în baza de date
        await _lessonRepository.AddAsync(lesson, ct);
    }
}