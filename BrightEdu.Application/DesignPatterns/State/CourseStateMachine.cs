using BrightEdu.Domain.Enums;

namespace BrightEdu.Application.DesignPatterns.State;

/// <summary>
/// Mașina de stări — gestionează tranziția unui curs între stări valide.
/// </summary>
public sealed class CourseStateMachine
{
    private ICourseStateHandler _current;

    public CourseStateMachine(CourseState initialState)
    {
        _current = initialState switch
        {
            CourseState.Draft => new DraftCourseStateHandler(),
            CourseState.Published => new PublishedCourseStateHandler(),
            CourseState.Archived => new ArchivedCourseStateHandler(),
            _ => new DraftCourseStateHandler()
        };
    }

    public CourseState CurrentState => _current.State;
    public bool CanPublish() => _current.CanPublish();
    public bool CanArchive() => _current.CanArchive();
    public bool CanRevertToDraft() => _current.CanRevertToDraft();

    public CourseState Publish()
    {
        _current = _current.Publish();
        return _current.State;
    }

    public CourseState Archive()
    {
        _current = _current.Archive();
        return _current.State;
    }

    public CourseState RevertToDraft()
    {
        _current = _current.RevertToDraft();
        return _current.State;
    }
}
