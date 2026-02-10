using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Interfaces;

// DIP: Application definește contractul, nu implementarea.
public interface ICourseRepository
{
    // Orice clasă care implementează ICourseRepository trebuie să ofere o metodă care întoarce toate cursurile, asincron, cu posibilitate de anulare.
    Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken ct = default);
}