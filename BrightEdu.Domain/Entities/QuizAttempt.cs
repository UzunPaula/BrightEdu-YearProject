namespace BrightEdu.Domain.Entities;

public class QuizAttempt
{
    private readonly List<QuizAttemptAnswer> _answers = new();

    private QuizAttempt()
    {
    }

    public QuizAttempt(Guid id, Guid quizId, Guid studentId, int attemptNumber)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        if (quizId == Guid.Empty) throw new ArgumentException("QuizId invalid.", nameof(quizId));
        if (studentId == Guid.Empty) throw new ArgumentException("StudentId invalid.", nameof(studentId));
        if (attemptNumber <= 0) throw new ArgumentOutOfRangeException(nameof(attemptNumber));

        Id = id;
        QuizId = quizId;
        StudentId = studentId;
        AttemptNumber = attemptNumber;
        StartedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid QuizId { get; private set; }
    public Quiz Quiz { get; private set; } = null!;
    public Guid StudentId { get; private set; }
    public User Student { get; private set; } = null!;
    public int AttemptNumber { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? SubmittedAt { get; private set; }
    public decimal? Score { get; private set; }
    public bool Passed { get; private set; }

    public IReadOnlyCollection<QuizAttemptAnswer> Answers => _answers.AsReadOnly();

    public void AddOrReplaceAnswer(Guid questionId, Guid? selectedOptionId, string? textAnswer)
    {
        if (SubmittedAt is not null)
            throw new InvalidOperationException("Încercarea a fost deja trimisă.");

        var existing = _answers.FirstOrDefault(x => x.QuestionId == questionId);
        if (existing is not null)
        {
            existing.Update(selectedOptionId, textAnswer);
            return;
        }

        _answers.Add(new QuizAttemptAnswer(Guid.NewGuid(), Id, questionId, selectedOptionId, textAnswer));
    }

    public void Submit(decimal score, bool passed)
    {
        if (SubmittedAt is not null)
            throw new InvalidOperationException("Încercarea a fost deja trimisă.");

        Score = score;
        Passed = passed;
        SubmittedAt = DateTime.UtcNow;
    }
}
