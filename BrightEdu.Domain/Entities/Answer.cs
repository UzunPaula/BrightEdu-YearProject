namespace BrightEdu.Domain.Entities;

public class Answer
{
    public Guid Id { get; private set; }
    public string Text { get; private set; }
    public bool IsCorrect { get; private set; }

    // Cheie străină către întrebarea din care face parte răspunsul.
    public Guid QuestionId { get; private set; }

    // Proprietate de navigare către întrebare.
    public Question Question { get; private set; } = null!;

    // Constructor privat necesar pentru EF Core.
    private Answer() { }

    // Constructorul principal pentru crearea unui răspuns valid.
    public Answer(Guid id, string text, bool isCorrect, Guid questionId)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id invalid.", nameof(id));

        if (questionId == Guid.Empty)
            throw new ArgumentException("QuestionId invalid.", nameof(questionId));

        Id = id;
        IsCorrect = isCorrect;
        QuestionId = questionId;

        SetText(text);
    }

    // Setează textul răspunsului cu validare.
    public void SetText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Textul răspunsului nu poate fi gol.", nameof(text));

        Text = text.Trim();
    }

    // Permite schimbarea valorii IsCorrect dacă va fi nevoie mai târziu.
    public void SetIsCorrect(bool isCorrect)
    {
        IsCorrect = isCorrect;
    }
}