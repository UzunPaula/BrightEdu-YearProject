using BrightEdu.Application.DesignPatterns.FactoryMethod.Steps;
using BrightEdu.Application.DTOs;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Infrastructure.DesignPatterns.FactoryMethod.Steps;

// Factory Method: creator concret pentru step-ul de tip "question".
// Aplicația lucrează cu ILessonStepCreator, nu cu new QuestionStep direct (DIP).
public sealed class QuestionStepCreator : ILessonStepCreator
{
    // Cheia folosită de resolver ca să aleagă acest creator.
    public string Type => "question";
    
    // Factory Method: construiește obiectul concret QuestionStep dintr-un DTO de input.
    public LessonStep Create(CreateLessonStepRequestDto dto)
    {
        // Validări ca să nu creez un step invalid.
        if (string.IsNullOrWhiteSpace(dto.QuestionText))
            throw new ArgumentException("QuestionText is required for question step.", nameof(dto));

        if (dto.Options is null || dto.Options.Count < 2)
            throw new ArgumentException("Options must have at least 2 items.", nameof(dto));

        if (dto.CorrectOptionIndex is null)
            throw new ArgumentException("CorrectOptionIndex is required for question step.", nameof(dto));

        // Creez entitatea Domain. Restul codului nu trebuie să știe constructorul lui QuestionStep.
        return new QuestionStep(dto.Id, dto.Order, dto.QuestionText, dto.Options, dto.CorrectOptionIndex.Value);
    }   
}

