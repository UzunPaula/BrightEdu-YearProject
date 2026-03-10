namespace BrightEdu.Domain.Prototype;

public interface IPrototype<T>
{
    // Creez o copie a obiectului curent
    T Clone();
}