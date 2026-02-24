using BrightEdu.Application.DTOs;

namespace BrightEdu.Application.Interfaces;

public interface ICreateLessonService
{
    Task<LessonDto> CreateAsync(CreateLessonRequestDto dto, CancellationToken ct);
}