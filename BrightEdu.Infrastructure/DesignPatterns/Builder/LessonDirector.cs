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

    public Lesson Construct(CreateLessonRequestDto dto)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        _lessonBuilder.Reset();

        var lesson = _lessonBuilder
            .WithId(Guid.NewGuid())
            .WithTitle(dto.Title)
            .WithCourseId(dto.CourseId)
            .WithOrder(dto.Order)
            .WithContent(dto.Content)
            .Build();

        _lessonBuilder.Reset();
        return lesson;
    }
}
