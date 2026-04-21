using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;

namespace BrightEdu.Application.DesignPatterns.Bridge;

public sealed class LessonBridgeService
{
    private readonly ILessonRepository _lessonRepository;

    public LessonBridgeService(ILessonRepository lessonRepository)
    {
        _lessonRepository = lessonRepository;
    }

    public async Task<LessonBridgeResultDto> RenderAsync(
        Guid lessonId,
        string displayType,
        bool includeQuiz,
        CancellationToken ct = default)
    {
        // Luăm lecția reală din baza de date
        var lesson = await _lessonRepository.GetByIdAsync(lessonId, ct);

        if (lesson is null)
            throw new Exception("Lecție inexistentă.");

        // Alegem implementarea
        ILessonDisplay display = displayType switch
        {
            "Simple" => new SimpleLessonDisplay(),
            "Detailed" => new DetailedLessonDisplay(),
            _ => throw new ArgumentException("Tip de afișare invalid.")
        };

        // Alegem abstracția
        LessonScreen screen = includeQuiz
            ? new LessonWithQuizScreen(display)
            : new LessonScreen(display);

        var result = screen.Render(lesson);

        return new LessonBridgeResultDto(result);
    }
}