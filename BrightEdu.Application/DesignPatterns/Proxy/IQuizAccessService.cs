using BrightEdu.Application.DTOs;

namespace BrightEdu.Application.DesignPatterns.Proxy;

public interface IQuizAccessService
{
    Task<QuizDetailsDto?> GetByIdAsync(Guid quizId, CancellationToken ct = default);
}