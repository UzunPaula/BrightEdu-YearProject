namespace BrightEdu.Application.DesignPatterns.Flyweight;

// Obiectul partajat care conține date comune pentru tipul întrebării
public sealed class QuestionTypeFlyweight : IQuestionTypeFlyweight
{
    public string TypeName { get; }
    public string Description { get; }
    public string DisplayRule { get; }

    public QuestionTypeFlyweight(string typeName, string description, string displayRule)
    {
        TypeName = typeName;
        Description = description;
        DisplayRule = displayRule;
    }
}