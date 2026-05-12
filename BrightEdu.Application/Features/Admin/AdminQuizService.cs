using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Domain.Enums;

namespace BrightEdu.Application.Features.Admin;

public interface IAdminQuizService
{
    Task<AdminQuizDto?> GetByLessonIdAsync(Guid lessonId, CancellationToken ct = default);
    Task<AdminQuizDto?> GetByModuleIdAsync(Guid moduleId, CancellationToken ct = default);
    Task<AdminQuizDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<AdminQuizDto> CreateForLessonAsync(CreateQuizForLessonRequest request, CancellationToken ct = default);
    Task<AdminQuizDto> CreateForModuleAsync(CreateQuizForModuleRequest request, CancellationToken ct = default);
    Task<AdminQuizDto> UpdateAsync(Guid id, UpdateQuizRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task PublishAsync(Guid id, CancellationToken ct = default);
}

public sealed class AdminQuizService : IAdminQuizService
{
    private readonly IAdminQuizRepository _repository;

    public AdminQuizService(IAdminQuizRepository repository)
    {
        _repository = repository;
    }

    public async Task<AdminQuizDto?> GetByLessonIdAsync(Guid lessonId, CancellationToken ct = default)
    {
        var quiz = await _repository.GetByLessonIdAsync(lessonId, ct);
        return quiz is null ? null : Map(quiz);
    }

    public async Task<AdminQuizDto?> GetByModuleIdAsync(Guid moduleId, CancellationToken ct = default)
    {
        var quiz = await _repository.GetByModuleIdAsync(moduleId, ct);
        return quiz is null ? null : Map(quiz);
    }

    public async Task<AdminQuizDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var quiz = await _repository.GetByIdAsync(id, ct);
        if (quiz is null) throw new KeyNotFoundException($"Quiz {id} negăsit.");
        return Map(quiz);
    }

    public async Task<AdminQuizDto> CreateForLessonAsync(CreateQuizForLessonRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Titlul quiz-ului este obligatoriu.");

        var quiz = Quiz.ForLesson(request.Title, request.LessonId);
        quiz.ConfigureRules(request.PassingScore, request.MaxAttempts, request.ShuffleQuestions, request.ShuffleAnswers, request.ShowMistakesAfterAttempt, request.ShowOnlyWrongAnswers, request.ShowCorrectAnswer);

        await _repository.AddAsync(quiz, ct);
        await _repository.SaveAsync(ct);

        return Map(quiz);
    }

    public async Task<AdminQuizDto> CreateForModuleAsync(CreateQuizForModuleRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Titlul quiz-ului este obligatoriu.");

        var quiz = Quiz.ForModule(request.Title, request.ModuleId);
        quiz.ConfigureRules(request.PassingScore, request.MaxAttempts, request.ShuffleQuestions, request.ShuffleAnswers, request.ShowMistakesAfterAttempt, request.ShowOnlyWrongAnswers, request.ShowCorrectAnswer);

        await _repository.AddAsync(quiz, ct);
        await _repository.SaveAsync(ct);

        return Map(quiz);
    }

    public async Task<AdminQuizDto> UpdateAsync(Guid id, UpdateQuizRequest request, CancellationToken ct = default)
    {
        var quiz = await _repository.GetByIdAsync(id, ct);
        if (quiz is null) throw new KeyNotFoundException($"Quiz {id} negăsit.");

        quiz.SetTitle(request.Title);
        quiz.ConfigureRules(request.PassingScore, request.MaxAttempts, request.ShuffleQuestions, request.ShuffleAnswers, request.ShowMistakesAfterAttempt, request.ShowOnlyWrongAnswers, request.ShowCorrectAnswer);

        await _repository.SaveAsync(ct);
        return Map(quiz);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var quiz = await _repository.GetByIdAsync(id, ct);
        if (quiz is null) throw new KeyNotFoundException($"Quiz {id} negăsit.");

        await _repository.RemoveAsync(quiz, ct);
    }

    public async Task PublishAsync(Guid id, CancellationToken ct = default)
    {
        var quiz = await _repository.GetByIdAsync(id, ct);
        if (quiz is null) throw new KeyNotFoundException($"Quiz {id} negăsit.");

        quiz.SetState(QuizState.Published);
        await _repository.SaveAsync(ct);
    }

    private static AdminQuizDto Map(Quiz quiz) =>
        new AdminQuizDto(
            quiz.Id,
            quiz.LessonId,
            quiz.ModuleId,
            quiz.Title,
            quiz.PassingScore,
            quiz.MaxAttempts,
            quiz.ShuffleQuestions,
            quiz.ShuffleAnswers,
            quiz.ShowMistakesAfterAttempt,
            quiz.ShowOnlyWrongAnswers,
            quiz.ShowCorrectAnswer,
            quiz.State.ToString(),
            quiz.Questions.Count);
}
