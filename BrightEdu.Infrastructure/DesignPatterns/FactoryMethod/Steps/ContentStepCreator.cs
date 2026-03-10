using BrightEdu.Application.DesignPatterns.FactoryMethod.Steps;
using BrightEdu.Application.DTOs;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Infrastructure.DesignPatterns.FactoryMethod.Steps;

// Factory Method: creator concret pentru step-ul de tip "content".
// Restul aplicației cere ILessonStepCreator, nu știe de ContentStep (DIP).
public sealed class ContentStepCreator : ILessonStepCreator
{
    // Cheia după care resolver-ul alege creatorul potrivit.
    public string Type => "content";

    // Factory Method: aici se construiește obiectul concret (ContentStep).
    public LessonStep Create(CreateLessonStepRequestDto dto)
    {
        // Validare minimă, ca să nu creez un step invalid.
        if (string.IsNullOrWhiteSpace(dto.Content))
            throw new ArgumentException("Content is required for content step.", nameof(dto));

        // Creez entitatea Domain fără ca alte clase să folosească new ContentStep direct.
        return new ContentStep(dto.Id, dto.Order, dto.Content);
    }
}

