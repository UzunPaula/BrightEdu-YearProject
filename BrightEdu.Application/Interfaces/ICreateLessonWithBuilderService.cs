using BrightEdu.Application.DTOs;

namespace BrightEdu.Application.Interfaces;

public interface ICreateLessonWithBuilderService
{
    Task CreateAsync(CreateLessonRequestDto dto, CancellationToken ct);
}