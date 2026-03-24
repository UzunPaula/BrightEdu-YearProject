using BrightEdu.Application.DTOs;

namespace BrightEdu.Application.Interfaces;

// Definesc serviciul care creează o lecție folosind Adapter.
public interface ICreateLessonWithAdapterService
{
    Task<LessonDto> CreateAsync(CreateLessonRequestDto request);
}