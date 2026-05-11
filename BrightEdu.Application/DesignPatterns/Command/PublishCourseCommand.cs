using BrightEdu.Application.DesignPatterns.State;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.DesignPatterns.Command;

/// <summary>
/// Command concret — publică un curs dacă starea curentă permite tranziția.
/// </summary>
public sealed class PublishCourseCommand : ICourseStateCommand
{
    private readonly Course _course;
    private readonly IAdminCourseRepository _repository;

    public PublishCourseCommand(Course course, IAdminCourseRepository repository)
    {
        _course = course;
        _repository = repository;
    }

    public string Name => $"Publică cursul '{_course.Title}'";

    public async Task ExecuteAsync(CancellationToken ct = default)
    {
        var machine = new CourseStateMachine(_course.State);

        if (!machine.CanPublish())
            throw new InvalidOperationException($"Cursul nu poate fi publicat din starea {_course.State}.");

        var newState = machine.Publish();
        _course.SetState(newState);
        await _repository.SaveAsync(ct);
    }
}
