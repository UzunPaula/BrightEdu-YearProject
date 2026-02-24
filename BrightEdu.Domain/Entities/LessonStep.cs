namespace BrightEdu.Domain.Entities;

public abstract class LessonStep
{
    public abstract string Type { get; }
    public Guid Id { get; private set; }
    public int Order { get; private set; }

    protected LessonStep(Guid id, int order)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id invalid.", nameof(id));
        if (order <= 0) throw new ArgumentException("Order trebuie să fie > 0.", nameof(order));

        Id = id;
        Order = order;
    }
}