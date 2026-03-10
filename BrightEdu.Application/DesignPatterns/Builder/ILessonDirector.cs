using BrightEdu.Application.DTOs;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.DesignPatterns.Builder;

public interface ILessonDirector
{
    // Construiește o lecție pe baza datelor primite prin DTO
    Lesson Construct(CreateLessonRequestDto dto);
}