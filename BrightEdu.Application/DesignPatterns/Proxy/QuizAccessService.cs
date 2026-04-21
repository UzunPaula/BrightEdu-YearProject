using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;

namespace BrightEdu.Application.DesignPatterns.Proxy;

public sealed class QuizAccessService : IQuizAccessService
{
    private readonly IQuizRepository _quizRepository;

    public QuizAccessService(IQuizRepository quizRepository)
    {
        _quizRepository = quizRepository;
    }

    public async Task<QuizDetailsDto?> GetByIdAsync(Guid quizId, CancellationToken ct = default)
    {
        var quiz = await _quizRepository.GetByIdAsync(quizId, ct);

        if (quiz is null)
            return null;

        return new QuizDetailsDto(
            quiz.Id,
            quiz.Title,
            quiz.LessonId);
    }
}