using BrightEdu.Application.DesignPatterns.FactoryMethod.Steps;
using BrightEdu.Application.DTOs;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Infrastructure.DesignPatterns.FactoryMethod.Steps;

public sealed class QuestionStepCreator : ILessonStepCreator
{
    public string Type => "question";
    public LessonStep Create(CreateLessonStepRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.QuestionText))
            throw new ArgumentException("QuestionText is required for question step.", nameof(dto));

        if (dto.Options is null || dto.Options.Count < 2)
            throw new ArgumentException("Options must have at least 2 items.", nameof(dto));

        if (dto.CorrectOptionIndex is null)
            throw new ArgumentException("CorrectOptionIndex is required for question step.", nameof(dto));

        return new QuestionStep(dto.Id, dto.Order, dto.QuestionText, dto.Options, dto.CorrectOptionIndex.Value);
    }   
}