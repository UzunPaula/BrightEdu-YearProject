using BrightEdu.Domain.Enums;

namespace BrightEdu.Domain.Entities;

public class User
{
    private readonly List<UserRole> _userRoles = new();
    private readonly List<RefreshToken> _refreshTokens = new();
    private readonly List<Enrollment> _enrollments = new();
    private readonly List<LessonProgress> _lessonProgresses = new();
    private readonly List<QuizAttempt> _quizAttempts = new();

    private User()
    {
    }

    public User(Guid id, string email, string passwordHash, string firstName, string lastName, LanguageCode preferredLanguage)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email invalid.", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("PasswordHash invalid.", nameof(passwordHash));

        Id = id;
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash.Trim();
        FirstName = string.IsNullOrWhiteSpace(firstName) ? throw new ArgumentException("FirstName invalid.", nameof(firstName)) : firstName.Trim();
        LastName = string.IsNullOrWhiteSpace(lastName) ? throw new ArgumentException("LastName invalid.", nameof(lastName)) : lastName.Trim();
        PreferredLanguage = preferredLanguage;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public LanguageCode PreferredLanguage { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();
    public IReadOnlyCollection<LessonProgress> LessonProgresses => _lessonProgresses.AsReadOnly();
    public IReadOnlyCollection<QuizAttempt> QuizAttempts => _quizAttempts.AsReadOnly();
}
