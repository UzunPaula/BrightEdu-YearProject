using System.Text.Json;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.DesignPatterns.Visitor;

/// <summary>
/// Visitor concret — validează că fiecare content block conține câmpurile obligatorii.
/// </summary>
public sealed class ContentBlockValidationVisitor : IContentBlockVisitor
{
    private readonly List<string> _errors = new();

    public IReadOnlyList<string> Errors => _errors.AsReadOnly();
    public bool IsValid => _errors.Count == 0;

    public void VisitText(LessonContentBlock block)
    {
        var config = ParseConfig(block);
        if (!config.TryGetValue("content", out var content) || string.IsNullOrWhiteSpace(content))
            _errors.Add($"Block Text (ord. {block.Order}): câmpul 'content' este obligatoriu.");
    }

    public void VisitImage(LessonContentBlock block)
    {
        var config = ParseConfig(block);
        if (!config.TryGetValue("url", out var url) || string.IsNullOrWhiteSpace(url))
            _errors.Add($"Block Image (ord. {block.Order}): câmpul 'url' este obligatoriu.");
    }

    public void VisitVideo(LessonContentBlock block)
    {
        var config = ParseConfig(block);
        if (!config.TryGetValue("url", out var url) || string.IsNullOrWhiteSpace(url))
            _errors.Add($"Block Video (ord. {block.Order}): câmpul 'url' este obligatoriu.");
    }

    public void VisitPdf(LessonContentBlock block)
    {
        var config = ParseConfig(block);
        if (!config.TryGetValue("url", out var url) || string.IsNullOrWhiteSpace(url))
            _errors.Add($"Block PdfEmbed (ord. {block.Order}): câmpul 'url' este obligatoriu.");
    }

    public void VisitCode(LessonContentBlock block)
    {
        var config = ParseConfig(block);
        if (!config.TryGetValue("language", out var lang) || string.IsNullOrWhiteSpace(lang))
            _errors.Add($"Block CodeEditor (ord. {block.Order}): câmpul 'language' este obligatoriu.");
    }

    private static Dictionary<string, string> ParseConfig(LessonContentBlock block)
    {
        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(block.ConfigJson)
                   ?? new Dictionary<string, string>();
        }
        catch
        {
            return new Dictionary<string, string>();
        }
    }
}
