using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Features.Lessons;

public interface ICreateLessonService
{
    Task<Guid> CreateAsync(CreateLessonRequestDto request, CancellationToken ct = default);
}

public sealed class CreateLessonService : ICreateLessonService
{
    private readonly ILessonRepository _lessonRepository;

    public CreateLessonService(ILessonRepository lessonRepository)
    {
        _lessonRepository = lessonRepository;
    }

    public async Task<Guid> CreateAsync(CreateLessonRequestDto request, CancellationToken ct = default)
    {
        var lesson = new Lesson(
            Guid.NewGuid(),
            request.Title,
            request.Content,
            request.Order,
            request.CourseId);

        await _lessonRepository.AddAsync(lesson, ct);

        return lesson.Id;
    }
}