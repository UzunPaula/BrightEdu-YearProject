using BrightEdu.Application.DesignPatterns.AbstractFactory.Steps;

namespace BrightEdu.Infrastructure.DesignPatterns.AbstractFactory.Steps;

// Resolver pentru Abstract Factory care primește stepType și întoarce fabrica potrivită din lista de fabrici disponibile.
public sealed class LessonStepAbstractFactoryResolver : ILessonStepAbstractFactoryResolver
{
    // DI îmi dă toate implementările de ILessonStepAbstractFactory(ContentLessonStepFactory și QuestionLessonStepFactory)
    private readonly IEnumerable<ILessonStepAbstractFactory> _factories;

    // Injectez colecția de fabrici ca sa pot adăuga fabrici noi fără să modific resolverul.
    public LessonStepAbstractFactoryResolver(IEnumerable<ILessonStepAbstractFactory> factories)
    {
        _factories = factories;
    }

    // Alege fabrica corectă după stepType si caut prima fabrică ce spune CanHandle(stepType) = true.
    public ILessonStepAbstractFactory Resolve(string stepType)
    {
        var factory = _factories.FirstOrDefault(f => f.CanHandle(stepType));
        // Dacă nu există fabrică pentru tip, arunc excepție care mă ajută să depistez rapid un input greșit sau un tip neimplementat.
        if (factory is null)
            throw new InvalidOperationException($"Unsupported step type: {stepType}");

        return factory;
    }
}