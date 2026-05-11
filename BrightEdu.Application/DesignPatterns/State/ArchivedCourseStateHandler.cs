using BrightEdu.Domain.Enums;

namespace BrightEdu.Application.DesignPatterns.State;

/// <summary>
/// State concret — cursul este arhivat (poate fi retras în Draft pentru revizuire).
/// </summary>
public sealed class ArchivedCourseStateHandler : ICourseStateHandler
{
    public CourseState State => CourseState.Archived;

    public bool CanPublish() => false;
    public bool CanArchive() => false;
    public bool CanRevertToDraft() => true;

    public ICourseStateHandler Publish() =>
        throw new InvalidOperationException("Un curs arhivat nu poate fi publicat direct. Revino la Draft mai întâi.");
    public ICourseStateHandler Archive() =>
        throw new InvalidOperationException("Cursul este deja arhivat.");
    public ICourseStateHandler RevertToDraft() => new DraftCourseStateHandler();
}
