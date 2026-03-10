using BrightEdu.Application.DesignPatterns.Builder;
using BrightEdu.Application.DTOs;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Infrastructure.DesignPatterns.Builder;

// Directorul orchestrează procesul de construcție folosind un builder.
// Izolează logica de construcție de client.
public class LessonDirector : ILessonDirector
{
    private readonly ILessonBuilder _lessonBuilder;

    public LessonDirector(ILessonBuilder lessonBuilder)
    {
        _lessonBuilder = lessonBuilder;
    }

    // Construiește o lecție pe baza DTO-ului primit.
    // Rulează pașii de construcție în ordinea corectă și validează datele de intrare.
    public Lesson Construct(CreateLessonRequestDto dto)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        // Reset pentru a începe cu o stare curată
        _lessonBuilder.Reset();
        _lessonBuilder
            .WithId(dto.Id)
            .WithTitle(dto.Title);

        foreach (var stepDto in dto.Steps.OrderBy(s => s.Order))
        {
            if (stepDto.Type.Equals("content", StringComparison.OrdinalIgnoreCase))
            {
                _lessonBuilder.AddContentStep(
                    stepDto.Id,
                    stepDto.Order,
                    stepDto.Content ?? throw new ArgumentException("Content lipsește pentru step de tip content."));
            }
            else if (stepDto.Type.Equals("question", StringComparison.OrdinalIgnoreCase))
            {
                _lessonBuilder.AddQuestionStep(
                    stepDto.Id,
                    stepDto.Order,
                    stepDto.QuestionText ?? throw new ArgumentException("QuestionText lipsește pentru step de tip question."),
                    stepDto.Options ?? throw new ArgumentException("Options lipsesc pentru step de tip question."),
                    stepDto.CorrectOptionIndex ?? throw new ArgumentException("CorrectOptionIndex lipsește pentru step de tip question."));
            }
            else
            {
                throw new ArgumentException($"Tip de step necunoscut: {stepDto.Type}");
            }
        }

        var lesson = _lessonBuilder.Build();
        _lessonBuilder.Reset();

        return lesson;
    }
}