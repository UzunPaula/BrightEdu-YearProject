using BrightEdu.Application.DesignPatterns.Adapter.Steps;
using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Features.Lessons;

public class CreateLessonWithAdapterService : ICreateLessonWithAdapterService
{
    private readonly ILessonCreationAdapter _adapter;
    private readonly ILessonRepository _lessonRepository;
    private readonly IEnumerable<ILessonStepDtoMapper> _lessonStepDtoMappers;

    public CreateLessonWithAdapterService(
        ILessonCreationAdapter adapter,
        ILessonRepository lessonRepository,
        IEnumerable<ILessonStepDtoMapper> lessonStepDtoMappers)
    {
        _adapter = adapter;
        _lessonRepository = lessonRepository;
        _lessonStepDtoMappers = lessonStepDtoMappers;
    }

    public async Task<LessonDto> CreateAsync(CreateLessonRequestDto request)
    {
        // Adapterul transformă requestul API într-un model comun.
        var adaptedLesson = _adapter.Adapt(request);

        // Creez entitatea Lesson pe baza modelului adaptat.
        var lesson = new Lesson(adaptedLesson.Id, adaptedLesson.Title);

        // Parcurg toți pașii adaptați și creez entitățile reale din Domain.
        foreach (var step in adaptedLesson.Steps)
        {
            LessonStep lessonStep;

            if (step.Type == "content")
            {
                lessonStep = new ContentStep(
                    step.Id,
                    step.Order,
                    step.Content ?? string.Empty);
            }
            else if (step.Type == "question")
            {
                lessonStep = new QuestionStep(
                    step.Id,
                    step.Order,
                    step.QuestionText ?? string.Empty,
                    step.Options ?? new List<string>(),
                    step.CorrectOptionIndex ?? 0);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported step type: {step.Type}");
            }
            // Adaug step-ul creat în lecție.
            lesson.AddStep(lessonStep);
        }
        // Salvez lecția în repository.
        await _lessonRepository.AddAsync(lesson);
        
        // Convertesc pașii lecției în DTO-uri pentru răspunsul final.
        var lessonStepDtos = lesson.Steps
            .Select(step =>
            {
                var mapper = _lessonStepDtoMappers.FirstOrDefault(m => m.CanMap(step));

                if (mapper is null)
                {
                    throw new InvalidOperationException($"No mapper found for step type: {step.Type}");
                }

                return mapper.Map(step);
            })
            .ToList();

        // Returnez lecția salvată în format DTO.
        return new LessonDto(
            lesson.Id,
            lesson.Title,
            lessonStepDtos);
    }
}