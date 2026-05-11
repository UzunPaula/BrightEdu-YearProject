namespace BrightEdu.Domain.Entities;

public class Category
{
    private readonly List<CourseCategory> _courseCategories = new();

    private Category()
    {
    }

    public Category(Guid id, string name)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name invalid.", nameof(name));

        Id = id;
        Name = name.Trim();
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;

    public IReadOnlyCollection<CourseCategory> CourseCategories => _courseCategories.AsReadOnly();
}
