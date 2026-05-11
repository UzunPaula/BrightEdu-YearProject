using BrightEdu.Domain.Enums;

namespace BrightEdu.Application.DesignPatterns.State;

/// <summary>
/// State concret — cursul este publicat (poate fi arhivat sau retras în Draft).
/// </summary>
public sealed class PublishedCourseStateHandler : ICourseStateHandler
{
    public CourseState State => CourseState.Published;

    public bool CanPublish() => false;
    public bool CanArchive() => true;
    public bool CanRevertToDraft() => true;

    public ICourseStateHandler Publish() =>
        throw new InvalidOperationException("Cursul este deja publicat.");
    public ICourseStateHandler Archive() => new ArchivedCourseStateHandler();
    public ICourseStateHandler RevertToDraft() => new DraftCourseStateHandler();
}
