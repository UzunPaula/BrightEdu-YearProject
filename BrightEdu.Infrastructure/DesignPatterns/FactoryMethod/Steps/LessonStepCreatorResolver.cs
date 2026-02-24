using BrightEdu.Application.DesignPatterns.FactoryMethod.Steps;

namespace BrightEdu.Infrastructure.DesignPatterns.FactoryMethod.Steps;

public class LessonStepCreatorResolver : ILessonStepCreatorResolver
{
    private readonly IEnumerable<ILessonStepCreator> _creators;

    public LessonStepCreatorResolver(IEnumerable<ILessonStepCreator> creators)
    {
        _creators = creators;
    }

    public ILessonStepCreator Resolve(string stepType)
    {
        var creator = _creators.FirstOrDefault(c =>
            string.Equals(c.Type, stepType, StringComparison.OrdinalIgnoreCase));

        if (creator is null)
            throw new InvalidOperationException($"Unsupported step type: {stepType}");

        return creator;
    }
}