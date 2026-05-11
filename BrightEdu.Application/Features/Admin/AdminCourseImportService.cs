using BrightEdu.Application.DTOs;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Domain.Enums;

namespace BrightEdu.Application.Features.Admin;

public interface IAdminCourseImportService
{
    Task<CourseImportResultDto> ImportAsync(CourseImportDto dto, CancellationToken ct = default);
}

public sealed class AdminCourseImportService : IAdminCourseImportService
{
    private readonly IAdminCourseRepository _courseRepo;
    private readonly IAdminModuleRepository _moduleRepo;
    private readonly IAdminLessonRepository _lessonRepo;
    private readonly IAdminQuizRepository _quizRepo;
    private readonly IAdminQuestionRepository _questionRepo;

    public AdminCourseImportService(
        IAdminCourseRepository courseRepo,
        IAdminModuleRepository moduleRepo,
        IAdminLessonRepository lessonRepo,
        IAdminQuizRepository quizRepo,
        IAdminQuestionRepository questionRepo)
    {
        _courseRepo = courseRepo;
        _moduleRepo = moduleRepo;
        _lessonRepo = lessonRepo;
        _quizRepo = quizRepo;
        _questionRepo = questionRepo;
    }

    public async Task<CourseImportResultDto> ImportAsync(CourseImportDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new ArgumentException("Titlul cursului este obligatoriu.");

        var course = new Course(Guid.NewGuid(), dto.Title, dto.ShortDescription);
        course.SetLevel(string.IsNullOrWhiteSpace(dto.Level) ? "Beginner" : dto.Level);
        course.AddTranslation(LanguageCode.Ro, dto.Title, dto.ShortDescription, dto.FullDescription);

        foreach (var t in dto.Translations ?? [])
        {
            var lc = ParseLang(t.Lang);
            if (lc != LanguageCode.Ro)
                course.AddTranslation(lc, t.Title, t.ShortDescription, t.FullDescription);
        }

        if (dto.Published)
            course.SetState(CourseState.Published);

        await _courseRepo.AddAsync(course, ct);
        await _courseRepo.SaveAsync(ct);

        int lessonsImported = 0;
        int quizzesImported = 0;

        foreach (var mDto in dto.Modules ?? [])
        {
            var module = new Module(Guid.NewGuid(), course.Id, mDto.Order, mDto.Title, mDto.Description);
            foreach (var t in mDto.Translations ?? [])
            {
                var lc = ParseLang(t.Lang);
                if (lc != LanguageCode.Ro)
                    module.AddTranslation(lc, t.Title, t.Description);
            }

            if (mDto.Published || dto.Published)
                module.SetState(CourseState.Published);

            await _moduleRepo.AddAsync(module, ct);
            await _moduleRepo.SaveAsync(ct);

            foreach (var lDto in mDto.Lessons ?? [])
            {
                var quiz = await ImportLessonAsync(lDto, course.Id, module.Id, lDto.Published || mDto.Published || dto.Published, ct);
                lessonsImported++;
                if (quiz) quizzesImported++;
            }
        }

        foreach (var lDto in dto.StandaloneLessons ?? [])
        {
            var quiz = await ImportLessonAsync(lDto, course.Id, null, lDto.Published || dto.Published, ct);
            lessonsImported++;
            if (quiz) quizzesImported++;
        }

        return new CourseImportResultDto(course.Id, dto.Title, dto.Modules?.Count ?? 0, lessonsImported, quizzesImported);
    }

    private async Task<bool> ImportLessonAsync(LessonImportDto lDto, Guid courseId, Guid? moduleId, bool publish, CancellationToken ct)
    {
        var lesson = new Lesson(Guid.NewGuid(), lDto.Title, lDto.Summary, lDto.Order, courseId);
        if (moduleId.HasValue) lesson.AssignModule(moduleId.Value);
        if (lDto.EstimatedMinutes > 0) lesson.SetEstimatedMinutes(lDto.EstimatedMinutes);
        lesson.EnableCodeEditor(lDto.CodeEditorEnabled);
        lesson.AddTranslation(LanguageCode.Ro, lDto.Title, lDto.Summary);

        foreach (var t in lDto.Translations ?? [])
        {
            var lc = ParseLang(t.Lang);
            if (lc != LanguageCode.Ro)
                lesson.AddTranslation(lc, t.Title, t.Summary);
        }

        if (publish)
            lesson.SetState(LessonState.Published);

        await _lessonRepo.AddAsync(lesson, ct);
        await _lessonRepo.SaveAsync(ct);

        foreach (var b in lDto.ContentBlocks ?? [])
        {
            if (!Enum.TryParse<ContentBlockType>(b.BlockType, ignoreCase: true, out var bt)) continue;
            _lessonRepo.AddContentBlock(new LessonContentBlock(Guid.NewGuid(), lesson.Id, b.Order, bt, b.ConfigJson, b.Lang));
        }

        if (lDto.ContentBlocks?.Any() == true)
            await _lessonRepo.SaveAsync(ct);

        if (lDto.Quiz is not null)
        {
            await ImportQuizAsync(lDto.Quiz, lesson.Id, publish || lDto.Quiz.Published, ct);
            return true;
        }

        return false;
    }

    private async Task ImportQuizAsync(QuizImportDto qDto, Guid lessonId, bool publish, CancellationToken ct)
    {
        var quiz = Quiz.ForLesson(qDto.Title, lessonId);
        quiz.ConfigureRules(qDto.PassingScore, qDto.MaxAttempts, qDto.ShuffleQuestions, qDto.ShuffleAnswers);

        if (publish)
            quiz.SetState(QuizState.Published);

        await _quizRepo.AddAsync(quiz, ct);
        await _quizRepo.SaveAsync(ct);

        foreach (var qsDto in qDto.Questions ?? [])
        {
            if (!Enum.TryParse<QuestionType>(qsDto.Type, ignoreCase: true, out var qt))
                qt = QuestionType.SingleChoice;

            var question = new Question(Guid.NewGuid(), qsDto.Text, quiz.Id);
            question.Configure(qt, qsDto.Order, qsDto.Points);

            var answers = qsDto.Answers
                .Select(a => {
                    var ans = new Answer(Guid.NewGuid(), a.Text, a.IsCorrect, question.Id);
                    ans.Configure(a.IsCorrect, a.Order > 0 ? a.Order : 1);
                    return ans;
                })
                .ToList();

            foreach (var ans in answers)
                question.AddAnswer(ans);

            await _questionRepo.AddAsync(question, ct);
        }

        await _questionRepo.SaveAsync(ct);
    }

    private static LanguageCode ParseLang(string lang) => lang.ToLowerInvariant() switch
    {
        "en" => LanguageCode.En,
        "ru" => LanguageCode.Ru,
        _ => LanguageCode.Ro
    };
}
