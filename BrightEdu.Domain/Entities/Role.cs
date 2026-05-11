namespace BrightEdu.Domain.Entities;

public class Role
{
    private readonly List<UserRole> _userRoles = new();

    private Role()
    {
    }

    public Role(Guid id, string name)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name invalid.", nameof(name));

        Id = id;
        Name = name.Trim();
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;

    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();
}
