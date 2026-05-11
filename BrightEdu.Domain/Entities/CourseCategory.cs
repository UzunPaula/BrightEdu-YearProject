namespace BrightEdu.Domain.Entities;

public class CourseCategory
{
    private CourseCategory()
    {
    }

    public CourseCategory(Guid courseId, Guid categoryId)
    {
        if (courseId == Guid.Empty) throw new ArgumentException("CourseId invalid.", nameof(courseId));
        if (categoryId == Guid.Empty) throw new ArgumentException("CategoryId invalid.", nameof(categoryId));

        CourseId = courseId;
        CategoryId = categoryId;
    }

    public Guid CourseId { get; private set; }
    public Course Course { get; private set; } = null!;
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;
}
