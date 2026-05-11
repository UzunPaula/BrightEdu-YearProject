namespace BrightEdu.Application.DesignPatterns.Command;

/// <summary>
/// Command — încapsulează o operație de schimbare a stării unui curs.
/// </summary>
public interface ICourseStateCommand
{
    string Name { get; }
    Task ExecuteAsync(CancellationToken ct = default);
}
