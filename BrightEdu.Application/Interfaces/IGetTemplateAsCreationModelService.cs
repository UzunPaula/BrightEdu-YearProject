using BrightEdu.Application.DTOs;

namespace BrightEdu.Application.Interfaces;

// Definește serviciul care adaptează un template la modelul comun al lecției.
public interface IGetTemplateAsCreationModelService
{
    LessonCreationModel GetById(Guid templateId);
}