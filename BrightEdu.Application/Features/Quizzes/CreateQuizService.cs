using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Features.Quizzes;

public interface ICreateQuizService
{
    Task<Guid> CreateAsync(CreateQuizRequestDto request, CancellationToken ct = default);
}

public sealed class CreateQuizService : ICreateQuizService
{
    private readonly IQuizRepository _quizRepository;

    public CreateQuizService(IQuizRepository quizRepository)
    {
        _quizRepository = quizRepository;
    }

    public async Task<Guid> CreateAsync(CreateQuizRequestDto request, CancellationToken ct = default)
    {
        var quiz = new Quiz(
            Guid.NewGuid(),
            request.Title,
            request.LessonId);

        await _quizRepository.AddAsync(quiz, ct);

        return quiz.Id;
    }
}