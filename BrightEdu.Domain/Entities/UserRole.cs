namespace BrightEdu.Domain.Entities;

public class UserRole
{
    private UserRole()
    {
    }

    public UserRole(Guid userId, Guid roleId)
    {
        if (userId == Guid.Empty) throw new ArgumentException("UserId invalid.", nameof(userId));
        if (roleId == Guid.Empty) throw new ArgumentException("RoleId invalid.", nameof(roleId));

        UserId = userId;
        RoleId = roleId;
    }

    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public Guid RoleId { get; private set; }
    public Role Role { get; private set; } = null!;
}
