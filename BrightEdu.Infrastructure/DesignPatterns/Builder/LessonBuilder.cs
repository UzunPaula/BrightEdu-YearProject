using BrightEdu.Application.DesignPatterns.Builder;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Infrastructure.DesignPatterns.Builder;

// Construiește un obiect Lesson pas cu pas, separând construcția de reprezentare.
public sealed class LessonBuilder : ILessonBuilder
{
    private Guid _id;
    private string _title = string.Empty;
    private Guid _courseId;
    private int _order;
    private string _content = string.Empty;

    public ILessonBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public ILessonBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public ILessonBuilder WithCourseId(Guid courseId)
    {
        _courseId = courseId;
        return this;
    }

    public ILessonBuilder WithOrder(int order)
    {
        _order = order;
        return this;
    }

    public ILessonBuilder WithContent(string content)
    {
        _content = content;
        return this;
    }

    public Lesson Build()
    {
        if (_id == Guid.Empty) throw new InvalidOperationException("Id-ul lecției nu a fost setat.");
        if (_courseId == Guid.Empty) throw new InvalidOperationException("CourseId-ul nu a fost setat.");
        if (_order <= 0) throw new InvalidOperationException("Order-ul lecției nu a fost setat.");

        return new Lesson(_id, _title, _content, _order, _courseId);
    }

    public void Reset()
    {
        _id = Guid.Empty;
        _title = string.Empty;
        _courseId = Guid.Empty;
        _order = 0;
        _content = string.Empty;
    }
}
