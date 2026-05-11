using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.DesignPatterns.Visitor;

/// <summary>
/// Visitor — definește operații ce pot fi aplicate pe diferite tipuri de content block,
/// fără a modifica clasele entităților.
/// </summary>
public interface IContentBlockVisitor
{
    void VisitText(LessonContentBlock block);
    void VisitImage(LessonContentBlock block);
    void VisitVideo(LessonContentBlock block);
    void VisitPdf(LessonContentBlock block);
    void VisitCode(LessonContentBlock block);
}
