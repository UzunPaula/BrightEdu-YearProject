namespace BrightEdu.Application.DesignPatterns.Flyweight;

// Factory care reutilizează obiectele partajate
public sealed class QuestionTypeFactory
{
    private readonly Dictionary<string, IQuestionTypeFlyweight> _types = new();

    public IQuestionTypeFlyweight GetQuestionType(string typeName)
    {
        // Dacă tipul există deja, îl reutilizăm
        if (_types.TryGetValue(typeName, out var existingType))
            return existingType;

        // Dacă nu există, îl creăm o singură dată
        IQuestionTypeFlyweight newType = typeName switch
        {
            "SingleChoice" => new QuestionTypeFlyweight(
                "SingleChoice",
                "Întrebare cu un singur răspuns corect.",
                "Utilizatorul poate selecta o singură variantă."),

            "MultipleChoice" => new QuestionTypeFlyweight(
                "MultipleChoice",
                "Întrebare cu mai multe răspunsuri corecte.",
                "Utilizatorul poate selecta mai multe variante."),

            "TrueFalse" => new QuestionTypeFlyweight(
                "TrueFalse",
                "Întrebare de tip adevărat sau fals.",
                "Utilizatorul selectează una din cele două opțiuni."),

            _ => throw new ArgumentException("Tip de întrebare necunoscut.")
        };

        _types[typeName] = newType;
        return newType;
    }

    // Metodă utilă pentru a demonstra câte obiecte partajate există
    public int Count => _types.Count;
}