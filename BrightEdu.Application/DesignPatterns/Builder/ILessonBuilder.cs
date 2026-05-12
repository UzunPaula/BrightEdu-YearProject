using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.DesignPatterns.Builder;

public interface ILessonBuilder
{
    ILessonBuilder WithId(Guid id);
    ILessonBuilder WithTitle(string title);
    ILessonBuilder WithCourseId(Guid courseId);
    ILessonBuilder WithOrder(int order);
    ILessonBuilder WithContent(string content);

    Lesson Build();
    void Reset();
}