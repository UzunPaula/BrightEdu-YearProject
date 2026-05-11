using BrightEdu.Domain.Entities;
using BrightEdu.Domain.Enums;
using BrightEdu.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BrightEdu.Infrastructure.Services;

public sealed class SampleDataSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly Pbkdf2PasswordHashService _passwordHashService;

    public SampleDataSeeder(AppDbContext dbContext, Pbkdf2PasswordHashService passwordHashService)
    {
        _dbContext = dbContext;
        _passwordHashService = passwordHashService;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        await _dbContext.Database.EnsureCreatedAsync(ct);

        if (!await _dbContext.Roles.AnyAsync(ct))
        {
            var adminRole = new Role(Guid.NewGuid(), "Admin");
            var studentRole = new Role(Guid.NewGuid(), "Student");
            await _dbContext.Roles.AddRangeAsync([adminRole, studentRole], ct);

            var adminUser = new User(
                Guid.NewGuid(),
                "admin@brightedu.local",
                _passwordHashService.Hash("Admin123!"),
                "BrightEdu",
                "Admin",
                LanguageCode.Ro);

            await _dbContext.Users.AddAsync(adminUser, ct);
            await _dbContext.UserRoles.AddAsync(new UserRole(adminUser.Id, adminRole.Id), ct);
        }

        if (!await _dbContext.Courses.AnyAsync(ct))
        {
            var course = new Course(Guid.NewGuid(), "C# Backend Foundations", "Bazele construirii unui backend modern in .NET.");
            course.SetSlug("csharp-backend-foundations");
            course.SetLevel("Beginner");
            course.AddTranslation(LanguageCode.En, "C# Backend Foundations", "Modern .NET backend basics.", "Learn APIs, architecture and SQL.");
            course.AddTranslation(LanguageCode.Ru, "Основы C# Backend", "Базовые принципы современного backend на .NET.", "Изучение API, архитектуры и SQL.");

            var module = new Module(Guid.NewGuid(), course.Id, 1, "Introducere in API design", "Primii pasi in Web API.");
            course.AddModule(module);

            var lesson = new Lesson(Guid.NewGuid(), "Ce este un Web API?", "Un Web API expune date si functionalitati prin HTTP.", 1, course.Id);
            lesson.AssignModule(module.Id);
            lesson.SetEstimatedMinutes(15);
            lesson.AddContentBlock(ContentBlockType.Video, 2, "{\"title\":\"Video intro\",\"url\":\"https://www.youtube.com/watch?v=dQw4w9WgXcQ\"}");
            lesson.AddContentBlock(ContentBlockType.PdfEmbed, 3, "{\"title\":\"Support PDF\",\"url\":\"/files/backend-intro.pdf\"}");
            lesson.EnableCodeEditor(true);
            course.AddLesson(lesson);
            module.AddLesson(lesson);

            var quiz = new Quiz(Guid.NewGuid(), "Quiz Web API Intro", lesson.Id);
            quiz.ConfigureRules(60, 0, false, false);
            quiz.SetState(QuizState.Published);

            var question = new Question(Guid.NewGuid(), "Care protocol este folosit cel mai frecvent de Web API-uri?", quiz.Id);
            question.Configure(QuestionType.SingleChoice, 1, 1);
            question.AddTranslation(LanguageCode.En, "Which protocol is most commonly used by Web APIs?", null);
            question.AddTranslation(LanguageCode.Ru, "Какой протокол чаще всего используется Web API?", null);
            question.AddAnswer(new Answer(Guid.NewGuid(), "HTTP", true, question.Id));
            question.AddAnswer(new Answer(Guid.NewGuid(), "FTP", false, question.Id));
            question.AddAnswer(new Answer(Guid.NewGuid(), "SMTP", false, question.Id));
            quiz.AddQuestion(question);

            lesson.SetState(LessonState.Published);
            course.SetState(CourseState.Published);

            await _dbContext.Courses.AddAsync(course, ct);
            await _dbContext.Modules.AddAsync(module, ct);
            await _dbContext.Lessons.AddAsync(lesson, ct);
            await _dbContext.Quizzes.AddAsync(quiz, ct);
        }
        else
        {
            var existingCourse = await _dbContext.Courses.FirstOrDefaultAsync(x => x.Slug == "c#-backend-foundations", ct);
            if (existingCourse is not null)
            {
                existingCourse.SetSlug("csharp-backend-foundations");
            }

            var existingQuiz = await _dbContext.Quizzes.FirstOrDefaultAsync(x => x.Title == "Quiz Web API Intro", ct);
            if (existingQuiz is not null && existingQuiz.MaxAttempts != 0)
            {
                existingQuiz.ConfigureRules(
                    existingQuiz.PassingScore,
                    0,
                    existingQuiz.ShuffleQuestions,
                    existingQuiz.ShuffleAnswers);
            }
        }

        await _dbContext.SaveChangesAsync(ct);
    }
}
