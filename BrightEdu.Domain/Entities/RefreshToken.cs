namespace BrightEdu.Domain.Entities;

public class RefreshToken
{
    private RefreshToken()
    {
    }

    public RefreshToken(Guid id, Guid userId, string token, DateTime expiresAt)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        if (userId == Guid.Empty) throw new ArgumentException("UserId invalid.", nameof(userId));
        if (string.IsNullOrWhiteSpace(token)) throw new ArgumentException("Token invalid.", nameof(token));

        Id = id;
        UserId = userId;
        Token = token.Trim();
        ExpiresAt = expiresAt;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string Token { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
}
