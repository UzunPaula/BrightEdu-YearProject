using BrightEdu.Domain.Enums;

namespace BrightEdu.Application.DesignPatterns.State;

/// <summary>
/// State — definește comportamentul unui curs în funcție de starea sa curentă.
/// </summary>
public interface ICourseStateHandler
{
    CourseState State { get; }
    bool CanPublish();
    bool CanArchive();
    bool CanRevertToDraft();
    ICourseStateHandler Publish();
    ICourseStateHandler Archive();
    ICourseStateHandler RevertToDraft();
}
