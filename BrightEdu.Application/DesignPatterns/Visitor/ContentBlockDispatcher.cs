using BrightEdu.Domain.Entities;
using BrightEdu.Domain.Enums;

namespace BrightEdu.Application.DesignPatterns.Visitor;

/// <summary>
/// Dispatcher — dirijează fiecare block către metoda corectă a visitor-ului,
/// evitând modificarea entității de domeniu.
/// </summary>
public static class ContentBlockDispatcher
{
    public static void Dispatch(LessonContentBlock block, IContentBlockVisitor visitor)
    {
        switch (block.BlockType)
        {
            case ContentBlockType.Text:       visitor.VisitText(block);  break;
            case ContentBlockType.Image:      visitor.VisitImage(block); break;
            case ContentBlockType.Video:      visitor.VisitVideo(block); break;
            case ContentBlockType.PdfEmbed:   visitor.VisitPdf(block);   break;
            case ContentBlockType.CodeEditor: visitor.VisitCode(block);  break;
        }
    }

    public static void DispatchAll(IEnumerable<LessonContentBlock> blocks, IContentBlockVisitor visitor)
    {
        foreach (var block in blocks)
            Dispatch(block, visitor);
    }
}
