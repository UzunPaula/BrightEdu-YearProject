using BrightEdu.Application.DTOs;

namespace BrightEdu.Application.DesignPatterns.Proxy;

public sealed class QuizAccessProxy : IQuizAccessService
{
    private readonly QuizAccessService _realService;

    public QuizAccessProxy(QuizAccessService realService)
    {
        _realService = realService;
    }

    public async Task<QuizDetailsDto?> GetByIdAsync(Guid quizId, CancellationToken ct = default)
    {
        if (!CheckAccess())
            throw new UnauthorizedAccessException("Acces interzis la acest quiz.");

        return await _realService.GetByIdAsync(quizId, ct);
    }

    private bool CheckAccess()
    {
        return true;
    }
}