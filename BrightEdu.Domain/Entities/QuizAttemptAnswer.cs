namespace BrightEdu.Domain.Entities;

public class QuizAttemptAnswer
{
    private QuizAttemptAnswer()
    {
    }

    public QuizAttemptAnswer(Guid id, Guid quizAttemptId, Guid questionId, Guid? selectedOptionId, string? textAnswer)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        if (quizAttemptId == Guid.Empty) throw new ArgumentException("QuizAttemptId invalid.", nameof(quizAttemptId));
        if (questionId == Guid.Empty) throw new ArgumentException("QuestionId invalid.", nameof(questionId));

        Id = id;
        QuizAttemptId = quizAttemptId;
        QuestionId = questionId;
        SelectedOptionId = selectedOptionId;
        TextAnswer = string.IsNullOrWhiteSpace(textAnswer) ? null : textAnswer.Trim();
    }

    public Guid Id { get; private set; }
    public Guid QuizAttemptId { get; private set; }
    public QuizAttempt QuizAttempt { get; private set; } = null!;
    public Guid QuestionId { get; private set; }
    public Question Question { get; private set; } = null!;
    public Guid? SelectedOptionId { get; private set; }
    public Answer? SelectedOption { get; private set; }
    public string? TextAnswer { get; private set; }

    public void Update(Guid? selectedOptionId, string? textAnswer)
    {
        SelectedOptionId = selectedOptionId;
        TextAnswer = string.IsNullOrWhiteSpace(textAnswer) ? null : textAnswer.Trim();
    }
}
