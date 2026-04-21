using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Interfaces;

public interface IQuestionRepository
{
    Task<Question?> GetByIdAsync(Guid id, CancellationToken ct = default);
}