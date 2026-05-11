using BrightEdu.Domain.Enums;

namespace BrightEdu.Application.DesignPatterns.State;

/// <summary>
/// State concret — cursul este în stare Draft (poate fi publicat sau arhivat).
/// </summary>
public sealed class DraftCourseStateHandler : ICourseStateHandler
{
    public CourseState State => CourseState.Draft;

    public bool CanPublish() => true;
    public bool CanArchive() => true;
    public bool CanRevertToDraft() => false;

    public ICourseStateHandler Publish() => new PublishedCourseStateHandler();
    public ICourseStateHandler Archive() => new ArchivedCourseStateHandler();
    public ICourseStateHandler RevertToDraft() =>
        throw new InvalidOperationException("Cursul este deja în starea Draft.");
}
