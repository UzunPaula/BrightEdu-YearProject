using BrightEdu.Application.DesignPatterns.AbstractFactory.Steps;

namespace BrightEdu.Infrastructure.DesignPatterns.AbstractFactory.Steps;

// Rezolvatorul alege fabrica potrivită pe baza tipului de lecție.
public sealed class LessonComponentFactoryResolver : ILessonComponentFactoryResolver
{
    private readonly IReadOnlyDictionary<string, ILessonComponentFactory> _factories;

    public LessonComponentFactoryResolver(IEnumerable<ILessonComponentFactory> factories)
    {
        _factories = factories.ToDictionary(f => f.FactoryType, StringComparer.OrdinalIgnoreCase);
    }

    public ILessonComponentFactory Resolve(string factoryType)
    {
        if (_factories.TryGetValue(factoryType, out var factory))
            return factory;

        throw new InvalidOperationException($"Nu există fabrică pentru tipul '{factoryType}'.");
    }
}
