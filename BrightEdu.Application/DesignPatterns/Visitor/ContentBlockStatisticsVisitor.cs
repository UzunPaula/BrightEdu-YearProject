using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.DesignPatterns.Visitor;

/// <summary>
/// Visitor concret — colectează statistici despre blocurile de conținut ale lecției.
/// </summary>
public sealed class ContentBlockStatisticsVisitor : IContentBlockVisitor
{
    public int TextBlockCount { get; private set; }
    public int ImageBlockCount { get; private set; }
    public int VideoBlockCount { get; private set; }
    public int PdfBlockCount { get; private set; }
    public int CodeBlockCount { get; private set; }

    public int TotalBlocks => TextBlockCount + ImageBlockCount + VideoBlockCount + PdfBlockCount + CodeBlockCount;
    public bool HasInteractiveContent => VideoBlockCount > 0 || CodeBlockCount > 0;

    public void VisitText(LessonContentBlock block)  => TextBlockCount++;
    public void VisitImage(LessonContentBlock block) => ImageBlockCount++;
    public void VisitVideo(LessonContentBlock block) => VideoBlockCount++;
    public void VisitPdf(LessonContentBlock block)   => PdfBlockCount++;
    public void VisitCode(LessonContentBlock block)  => CodeBlockCount++;
}
