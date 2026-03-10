namespace BrightEdu.Application.Interfaces;

public interface ICreateLessonFromTemplateService
{
    Task<Guid> CreateFromTemplateAsync(Guid templateId, CancellationToken ct);
}