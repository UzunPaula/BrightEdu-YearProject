using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Features.Lessons;

public interface IGetLessonService
{
    Task<LessonDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
}

// SRP: use case pentru citirea unei lecții + transformarea în DTO.
// DIP: depinde de ILessonRepository și de o colecție de mapper-e (interfețe), nu de implementări concrete.
// OCP: adaug un tip nou de step printr-un mapper nou, fără să modific acest service.
public sealed class GetLessonService : IGetLessonService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IEnumerable<ILessonStepDtoMapper> _mappers;

    public GetLessonService(ILessonRepository lessonRepository, IEnumerable<ILessonStepDtoMapper> mappers)
    {
        _lessonRepository = lessonRepository;
        _mappers = mappers;
    }

    public async Task<LessonDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id, ct);
        if (lesson is null) return null;

        var steps = lesson.Steps.Select(MapStep).ToList();
        return new LessonDto(lesson.Id, lesson.Title, steps);
    }
    
    private LessonStepDto MapStep(BrightEdu.Domain.Entities.LessonStep step)
    {
        var mapper = _mappers.FirstOrDefault(m => m.CanMap(step));
        if (mapper is null) throw new NotSupportedException($"Nu există mapper pentru {step.GetType().Name}");
        return mapper.Map(step);
    }
}