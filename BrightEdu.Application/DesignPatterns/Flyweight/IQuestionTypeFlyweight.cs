namespace BrightEdu.Application.DesignPatterns.Flyweight;

// Interfața pentru obiectul partajat
public interface IQuestionTypeFlyweight
{
    string TypeName { get; }
    string Description { get; }
    string DisplayRule { get; }
}