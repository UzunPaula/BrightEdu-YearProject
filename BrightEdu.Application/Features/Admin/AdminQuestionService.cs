using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Domain.Enums;

namespace BrightEdu.Application.Features.Admin;

public interface IAdminQuestionService
{
    Task<IReadOnlyList<AdminQuestionDto>> GetByQuizIdAsync(Guid quizId, CancellationToken ct = default);
    Task<AdminQuestionDto> CreateAsync(CreateQuestionRequest request, CancellationToken ct = default);
    Task<AdminQuestionDto> UpdateAsync(Guid id, UpdateQuestionRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public sealed class AdminQuestionService : IAdminQuestionService
{
    private readonly IAdminQuestionRepository _repository;

    public AdminQuestionService(IAdminQuestionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<AdminQuestionDto>> GetByQuizIdAsync(Guid quizId, CancellationToken ct = default)
    {
        var questions = await _repository.GetByQuizIdAsync(quizId, ct);
        return questions.OrderBy(x => x.Order).Select(Map).ToList().AsReadOnly();
    }

    public async Task<AdminQuestionDto> CreateAsync(CreateQuestionRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            throw new ArgumentException("Textul întrebării este obligatoriu.");

        var question = new Question(Guid.NewGuid(), request.Text, request.QuizId);
        var type = Enum.Parse<QuestionType>(request.Type);
        question.Configure(type, request.Order, request.Points);

        foreach (var answerInput in request.Answers)
        {
            var answer = new Answer(Guid.NewGuid(), answerInput.Text, answerInput.IsCorrect, question.Id);
            answer.Configure(answerInput.IsCorrect, answerInput.Order);
            question.AddAnswer(answer);
        }

        await _repository.AddAsync(question, ct);
        await _repository.SaveAsync(ct);

        return Map(question);
    }

    public async Task<AdminQuestionDto> UpdateAsync(Guid id, UpdateQuestionRequest request, CancellationToken ct = default)
    {
        var question = await _repository.GetByIdWithAnswersAsync(id, ct);
        if (question is null) throw new KeyNotFoundException($"Întrebarea {id} negăsită.");

        question.UpdateText(request.Text);
        var type = Enum.Parse<QuestionType>(request.Type);
        question.Configure(type, request.Order, request.Points);

        var newAnswers = request.Answers.Select(a =>
        {
            var answer = new Answer(Guid.NewGuid(), a.Text, a.IsCorrect, question.Id);
            answer.Configure(a.IsCorrect, a.Order);
            return answer;
        }).ToList();

        await _repository.ReplaceAnswersAsync(question.Id, newAnswers, ct);
        await _repository.SaveAsync(ct);

        // Reload to get updated answers
        var updated = await _repository.GetByIdWithAnswersAsync(id, ct);
        return Map(updated!);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var question = await _repository.GetByIdWithAnswersAsync(id, ct);
        if (question is null) throw new KeyNotFoundException($"Întrebarea {id} negăsită.");

        await _repository.RemoveAsync(question, ct);
        await _repository.SaveAsync(ct);
    }

    private static AdminQuestionDto Map(Question question) =>
        new AdminQuestionDto(
            question.Id,
            question.QuizId,
            question.Text,
            question.Type.ToString(),
            question.Order,
            question.Points,
            question.Answers.Select(a => new AdminAnswerDto(a.Id, a.Text, a.IsCorrect, a.Order)).ToList().AsReadOnly());
}
