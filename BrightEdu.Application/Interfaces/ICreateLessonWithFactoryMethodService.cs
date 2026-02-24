using BrightEdu.Application.DTOs;

namespace BrightEdu.Application.Interfaces;

public interface ICreateLessonWithFactoryMethodService
{
    Task CreateAsync(CreateLessonRequestDto dto, CancellationToken ct);
}