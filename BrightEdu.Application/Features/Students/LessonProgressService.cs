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
        var (progress, lesson) = await GetOrCreateAsync(studentId, lessonId, ct);
        progress.MarkOpened();
        await _lessonProgressRepository.SaveChangesAsync(ct);
        return Map(progress, lesson);
    }

    public async Task<LessonProgressDto> UpdateAsync(Guid studentId, UpdateLessonProgressRequestDto request, CancellationToken ct = default)
    {
        var (progress, lesson) = await GetOrCreateAsync(studentId, request.LessonId, ct);
        var wasCompleted = progress.IsCompleted;
        progress.MarkCompleted(request.IsCompleted);
        await _lessonProgressRepository.SaveChangesAsync(ct);

        // Progresul cursului este calculat dinamic în StudentDashboardService — nu necesită notificare explicită.

        return Map(progress, lesson);
    }

    private async Task<(LessonProgress, Lesson)> GetOrCreateAsync(Guid studentId, Guid lessonId, CancellationToken ct)
    {
        var lesson = await _lessonRepository.GetByIdAsync(lessonId, ct);
        if (lesson is null)
            throw new InvalidOperationException("Lecția nu există.");

        var progress = await _lessonProgressRepository.GetAsync(studentId, lessonId, ct);
        if (progress is not null)
            return (progress, lesson);

        progress = new LessonProgress(Guid.NewGuid(), studentId, lessonId);
        await _lessonProgressRepository.AddAsync(progress, ct);
        // Re-fetch after insert: handles race condition where AddAsync swallowed a duplicate
        // key exception (two concurrent requests) and cleared the tracker.
        return ((await _lessonProgressRepository.GetAsync(studentId, lessonId, ct))!, lesson);
    }

    private static LessonProgressDto Map(LessonProgress progress, Lesson lesson)
        => new(
            progress.LessonId,
            lesson.Title,
            lesson.Course?.Slug,
            lesson.Course?.Title,
            progress.IsCompleted,
            progress.CompletedAt,
            progress.LastOpenedAt);
}
