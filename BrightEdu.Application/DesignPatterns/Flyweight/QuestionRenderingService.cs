using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;

namespace BrightEdu.Application.DesignPatterns.Flyweight;

public sealed class QuestionRenderingService
{
    private readonly QuestionTypeFactory _factory;
    private readonly IQuestionRepository _questionRepository;

    public QuestionRenderingService(
        QuestionTypeFactory factory,
        IQuestionRepository questionRepository)
    {
        _factory = factory;
        _questionRepository = questionRepository;
    }

    public async Task<RenderedQuestionDto> RenderAsync(Guid questionId, CancellationToken ct = default)
    {
        // Luăm întrebarea din DB prin repository
        var question = await _questionRepository.GetByIdAsync(questionId, ct);

        if (question is null)
            throw new Exception("Întrebare inexistentă.");

        // Luăm tipul partajat
        var type = _factory.GetQuestionType(question.Type.ToString());

        // Combinăm datele
        return new RenderedQuestionDto(
            question.Text,
            type.TypeName,
            type.Description,
            type.DisplayRule);
    }
}
