using BrightEdu.Application.DesignPatterns.AbstractFactory.Steps;

namespace BrightEdu.Infrastructure.DesignPatterns.AbstractFactory.Steps;

public sealed class LessonStepAbstractFactoryResolver : ILessonStepAbstractFactoryResolver
{
    private readonly IEnumerable<ILessonStepAbstractFactory> _factories;

    public LessonStepAbstractFactoryResolver(IEnumerable<ILessonStepAbstractFactory> factories)
    {
        _factories = factories;
    }

    public ILessonStepAbstractFactory Resolve(string stepType)
    {
        var factory = _factories.FirstOrDefault(f => f.CanHandle(stepType));
        if (factory is null)
            throw new InvalidOperationException($"Unsupported step type: {stepType}");

        return factory;
    }
}