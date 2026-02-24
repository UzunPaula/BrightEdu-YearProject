namespace BrightEdu.Application.DTOs;

// CourseDto este modelul pe care îl trimit prin API, ca să nu expun direct entitatea Course din Domain.
// Conține doar datele necesare pentru răspuns: Id, Title și Description.
public sealed record CourseDto(Guid Id, string Title, string? Description); 