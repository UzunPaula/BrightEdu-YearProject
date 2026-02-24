namespace BrightEdu.Application.DTOs;

// Abstracție + polimorfism: DTO de bază pentru orice tip de step dintr-o lecție.
// Type îți spune ce fel de step este, Id identifică step-ul, Order stabilește ordinea în lecție.
public abstract record LessonStepDto(string Type, Guid Id, int Order);

// Moștenire: DTO concret pentru step de tip conținut.
// Setează automat Type = "content" și adaugă câmpul Content.
public sealed record ContentStepDto(Guid Id, int Order, string Content)
    : LessonStepDto("content", Id, Order);

// Moștenire: DTO concret pentru step de tip întrebare.
// Setează automat Type = "question" și adaugă întrebarea și opțiunile.
public sealed record QuestionStepDto(Guid Id, int Order, string QuestionText, IReadOnlyList<string> Options, int CorrectOptionIndex)
    : LessonStepDto("question", Id, Order);