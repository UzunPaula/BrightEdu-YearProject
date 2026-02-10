namespace BrightEdu.Domain.Entities;

public sealed class QuestionStep : LessonStep
{
    public string QuestionText { get; private set; }
    public IReadOnlyList<string> Options { get; private set; }
    public int CorrectOptionIndex { get; private set; }
    
    public QuestionStep(Guid id, int order, string questionText,
        IReadOnlyList<string> options,
        int correctOptionIndex) : base(id, order)
    {
        if (string.IsNullOrWhiteSpace(questionText))
            throw new ArgumentException("Întrebarea nu poate fi goală.", nameof(questionText));

        if (options is null || options.Count < 2)
            throw new ArgumentException("Trebuie cel puțin 2 opțiuni.", nameof(options));

        if (correctOptionIndex < 0 || correctOptionIndex >= options.Count)
            throw new ArgumentException("Index corect invalid.", nameof(correctOptionIndex));

        QuestionText = questionText.Trim();
        Options = options;
        CorrectOptionIndex = correctOptionIndex;
    }
}