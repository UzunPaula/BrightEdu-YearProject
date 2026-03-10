using BrightEdu.Application.DesignPatterns.FactoryMethod.Steps;

namespace BrightEdu.Infrastructure.DesignPatterns.FactoryMethod.Steps;

// Resolver: alege creatorul corect în funcție de stepType (ex: "content", "question").
// Factory Method: separ logica de selecție de logica de creare a obiectului.
public class LessonStepCreatorResolver : ILessonStepCreatorResolver
{
    // Primesc toți creatorii înregistrați prin DI, ca să pot adăuga tipuri noi fără să modific resolverul (OCP).
    private readonly IEnumerable<ILessonStepCreator> _creators;

    public LessonStepCreatorResolver(IEnumerable<ILessonStepCreator> creators)
    {
        _creators = creators;
    }
    
    public ILessonStepCreator Resolve(string stepType)
    {
        // Caut creatorul care declară același Type (case-insensitive).
        var creator = _creators.FirstOrDefault(c =>
            string.Equals(c.Type, stepType, StringComparison.OrdinalIgnoreCase));

        // Dacă nu există, înseamnă că nu am implementat acel tip de step.
        if (creator is null)
            throw new InvalidOperationException($"Unsupported step type: {stepType}");

        // Întorc creatorul potrivit, iar el va crea entitatea Domain.
        return creator;
    }
}

