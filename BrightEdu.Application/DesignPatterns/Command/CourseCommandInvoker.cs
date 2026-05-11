namespace BrightEdu.Application.DesignPatterns.Command;

/// <summary>
/// Invoker — execută comenzi și menține istoricul operațiilor efectuate.
/// </summary>
public sealed class CourseCommandInvoker
{
    private readonly List<string> _executedCommands = new();

    public IReadOnlyList<string> History => _executedCommands.AsReadOnly();

    public async Task ExecuteAsync(ICourseStateCommand command, CancellationToken ct = default)
    {
        await command.ExecuteAsync(ct);
        _executedCommands.Add(command.Name);
    }
}
