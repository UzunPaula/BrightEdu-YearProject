using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;

namespace BrightEdu.Application.Features.Lessons;

// Interfață pentru serviciul care returnează o lecție după Id.
public interface IGetLessonService
{
    Task<LessonDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
}

// Serviciu simplu care citește o lecție din repository și o transformă în DTO.
public sealed class GetLessonService : IGetLessonService
{
    private readonly ILessonRepository _lessonRepository;

    public GetLessonService(ILessonRepository lessonRepository)
    {
        _lessonRepository = lessonRepository;
    }

    public async Task<LessonDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        // Citim lecția din repository.
        var lesson = await _lessonRepository.GetByIdAsync(id, ct);

        // Dacă lecția nu există, întoarcem null.
        if (lesson is null)
            return null;

        // Transformăm entitatea Domain în DTO.
        QuizDto? quizDto = null;

        if (lesson.Quiz is not null)
        {
            quizDto = new QuizDto(
                lesson.Quiz.Id,
                lesson.Quiz.Title,
                lesson.Quiz.Questions
                    .Select(question => new QuestionDto(
                        question.Id,
                        question.Text,
                        question.Answers
                            .Select(answer => new AnswerDto(
                                answer.Id,
                                answer.Text,
                                answer.IsCorrect))
                            .ToList()))
                    .ToList());
        }

        return new LessonDto(
            lesson.Id,
            lesson.Title,
            lesson.Content,
            lesson.Order,
            lesson.CourseId,
            quizDto);
    }
}