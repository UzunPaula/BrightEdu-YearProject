using BrightEdu.Application.DTOs;

namespace BrightEdu.Application.Interfaces;

public interface ICreateLessonWithAbstractFactoryService
{
    Task CreateAsync(CreateLessonRequestDto dto, CancellationToken ct);
}