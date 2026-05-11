using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Features.Students;

public interface ILessonProgressService
{
    Task<LessonProgressDto> MarkOpenedAsync(Guid studentId, Guid lessonId, CancellationToken ct = default);
    Task<LessonProgressDto> UpdateAsync(Guid studentId, UpdateLessonProgressRequestDto request, CancellationToken ct = default);
}

public sealed class LessonProgressService : ILessonProgressService
{
    private readonly ILessonProgressRepository _lessonProgressRepository;
    private readonly ILessonRepository _lessonRepository;

    public LessonProgressService(
        ILessonProgressRepository lessonProgressRepository,
        ILessonRepository lessonRepository)
    {
        _lessonProgressRepository = lessonProgressRepository;
        _lessonRepository = lessonRepository;
    }

    public async Task<LessonProgressDto> MarkOpenedAsync(Guid studentId, Guid lessonId, CancellationToken ct = default)
    {
        var progress = await GetOrCreateAsync(studentId, lessonId, ct);
        progress.MarkOpened();
        await _lessonProgressRepository.SaveChangesAsync(ct);
        return Map(progress);
    }

    public async Task<LessonProgressDto> UpdateAsync(Guid studentId, UpdateLessonProgressRequestDto request, CancellationToken ct = default)
    {
        var progress = await GetOrCreateAsync(studentId, request.LessonId, ct);
        progress.MarkCompleted(request.IsCompleted);
        await _lessonProgressRepository.SaveChangesAsync(ct);
        return Map(progress);
    }

    private async Task<LessonProgress> GetOrCreateAsync(Guid studentId, Guid lessonId, CancellationToken ct)
    {
        var lesson = await _lessonRepository.GetByIdAsync(lessonId, ct);
        if (lesson is null)
            throw new InvalidOperationException("Lecția nu există.");

        var progress = await _lessonProgressRepository.GetAsync(studentId, lessonId, ct);
        if (progress is not null)
            return progress;

        progress = new LessonProgress(Guid.NewGuid(), studentId, lessonId);
        await _lessonProgressRepository.AddAsync(progress, ct);
        return progress;
    }

    private static LessonProgressDto Map(LessonProgress progress)
        => new(
            progress.LessonId,
            progress.Lesson.Title,
            progress.Lesson.Course?.Slug,
            progress.IsCompleted,
            progress.CompletedAt,
            progress.LastOpenedAt);
}
