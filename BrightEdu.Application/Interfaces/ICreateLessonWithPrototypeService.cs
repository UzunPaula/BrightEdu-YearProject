namespace BrightEdu.Application.Interfaces;

public interface ICreateLessonWithPrototypeService
{
    Task<Guid> CreateFromPrototypeAsync(Guid sourceLessonId, CancellationToken ct);
}