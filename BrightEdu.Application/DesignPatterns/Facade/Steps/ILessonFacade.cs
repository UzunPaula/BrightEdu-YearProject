using BrightEdu.Application.DTOs;

namespace BrightEdu.Application.DesignPatterns.Facade.Steps;

// Interfață simplă pentru operațiile principale legate de lecții.
public interface ILessonFacade
{
    Task<LessonDto> CreateLessonFromRequestAsync(CreateLessonRequestDto request);
    LessonCreationModel GetLessonCreationModelFromTemplate(Guid templateId);
}