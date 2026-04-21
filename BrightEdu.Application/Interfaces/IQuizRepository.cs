using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Interfaces;

public interface IQuizRepository
{
    Task AddAsync(Quiz quiz, CancellationToken ct = default);
    Task<Quiz?> GetByIdAsync(Guid id, CancellationToken ct = default);
}