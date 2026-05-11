using BrightEdu.Application.DesignPatterns.State;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.DesignPatterns.Command;

/// <summary>
/// Command concret — arhivează un curs dacă starea curentă permite tranziția.
/// </summary>
public sealed class ArchiveCourseCommand : ICourseStateCommand
{
    private readonly Course _course;
    private readonly IAdminCourseRepository _repository;

    public ArchiveCourseCommand(Course course, IAdminCourseRepository repository)
    {
        _course = course;
        _repository = repository;
    }

    public string Name => $"Arhivează cursul '{_course.Title}'";

    public async Task ExecuteAsync(CancellationToken ct = default)
    {
        var machine = new CourseStateMachine(_course.State);

        if (!machine.CanArchive())
            throw new InvalidOperationException($"Cursul nu poate fi arhivat din starea {_course.State}.");

        var newState = machine.Archive();
        _course.SetState(newState);
        await _repository.SaveAsync(ct);
    }
}
