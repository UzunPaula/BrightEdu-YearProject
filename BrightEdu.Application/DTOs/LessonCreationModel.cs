namespace BrightEdu.Application.DTOs;

// Model comun al lecției, obținut după adaptare.
public class LessonCreationModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    // Lista de pași ai lecției în format comun.
    public List<LessonCreationStepModel> Steps { get; set; } = new();
}

// Model comun pentru un pas al lecției.
public class LessonCreationStepModel
{
    public string Type { get; set; } = string.Empty;
    public Guid Id { get; set; }
    public int Order { get; set; }

    // Câmp folosit pentru step de tip content.
    public string? Content { get; set; }

    // Câmp folosit pentru step de tip content.
    public string? QuestionText { get; set; }
    public List<string>? Options { get; set; }
    public int? CorrectOptionIndex { get; set; }
}