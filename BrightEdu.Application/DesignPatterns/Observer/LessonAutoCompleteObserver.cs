using BrightEdu.Application.Interfaces;

namespace BrightEdu.Application.DesignPatterns.Observer;

/// <summary>
/// Observer concret — marchează automat lecția ca finalizată când studentul trece quiz-ul.
/// </summary>
public sealed class LessonAutoCompleteObserver : IQuizResultObserver
{
    private readonly ILessonProgressRepository _progressRepository;

    public LessonAutoCompleteObserver(ILessonProgressRepository progressRepository)
    {
        _progressRepository = progressRepository;
    }

    public async Task OnQuizSubmittedAsync(Guid studentId, Guid? lessonId, bool passed, CancellationToken ct = default)
    {
        // Notificarea se procesează doar dacă quiz-ul a fost trecut și lecția există
        if (!passed || lessonId is null || lessonId == Guid.Empty)
            return;

        var progress = await _progressRepository.GetAsync(studentId, lessonId.Value, ct);
        if (progress is null || progress.IsCompleted)
            return;

        progress.MarkCompleted(true);
        await _progressRepository.SaveChangesAsync(ct);
    }
}
