using BrightEdu.Application.DesignPatterns.FactoryMethod.Steps;
using BrightEdu.Domain.Enums;

namespace BrightEdu.Infrastructure.DesignPatterns.FactoryMethod.Steps;

// Rezolvatorul alege creator-ul potrivit pe baza tipului de bloc.
public sealed class LessonContentBlockCreatorResolver : ILessonContentBlockCreatorResolver
{
    private readonly IReadOnlyDictionary<ContentBlockType, ILessonContentBlockCreator> _creators;

    public LessonContentBlockCreatorResolver(IEnumerable<ILessonContentBlockCreator> creators)
    {
        _creators = creators.ToDictionary(c => c.BlockType);
    }

    public ILessonContentBlockCreator Resolve(ContentBlockType blockType)
    {
        if (_creators.TryGetValue(blockType, out var creator))
            return creator;

        throw new InvalidOperationException($"Nu există creator pentru tipul de bloc '{blockType}'.");
    }
}
