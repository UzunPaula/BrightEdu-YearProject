using BrightEdu.Application.DesignPatterns.FactoryMethod.Steps;
using BrightEdu.Application.DTOs;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Infrastructure.DesignPatterns.FactoryMethod.Steps;

public sealed class ContentStepCreator : ILessonStepCreator
{
    public string Type => "content";

    public LessonStep Create(CreateLessonStepRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Content))
            throw new ArgumentException("Content is required for content step.", nameof(dto));

        return new ContentStep(dto.Id, dto.Order, dto.Content);
    }
}