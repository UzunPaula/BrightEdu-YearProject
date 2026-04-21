using BrightEdu.Application.DesignPatterns.Bridge;
using BrightEdu.Application.DesignPatterns.Decorator;
using BrightEdu.Application.DesignPatterns.Flyweight;
using BrightEdu.Application.DesignPatterns.Proxy;
using BrightEdu.Application.Features.Courses;
using BrightEdu.Application.Features.Lessons;
using BrightEdu.Application.Features.Quizzes;
using BrightEdu.Application.Interfaces;
using BrightEdu.Infrastructure.DataAccess;
using BrightEdu.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Adăugăm serviciile necesare pentru API și Swagger.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Înregistrăm repository-urile in-memory.
// Acestea simulează temporar accesul la date până conectăm EF Core.    
builder.Services.AddScoped<ICourseReadRepository, CourseRepository>();
builder.Services.AddScoped<ICourseWriteRepository, CourseRepository>();
builder.Services.AddScoped<ILessonRepository, LessonRepository>();
builder.Services.AddScoped<IQuizRepository, QuizRepository>();

// Înregistrăm serviciul pentru citirea cursurilor.
builder.Services.AddScoped<IGetCoursesService, GetCoursesService>();
builder.Services.AddScoped<IGetLessonService, GetLessonService>();
builder.Services.AddScoped<ICreateLessonService, CreateLessonService>();
builder.Services.AddScoped<ICreateQuizService, CreateQuizService>();

builder.Services.AddScoped<QuizAccessService>();
builder.Services.AddScoped<IQuizAccessService, QuizAccessProxy>();

// Înregistrăm serviciul de bază care calculează scorul quiz-ului
builder.Services.AddScoped<QuizEvaluationService>();
// Înregistrăm interfața folosind Decorator
builder.Services.AddScoped<IQuizEvaluationService>(sp =>
{
    // Luăm serviciul real din DI
    var baseService = sp.GetRequiredService<QuizEvaluationService>();
    // Adăugăm validarea peste serviciul real
    var validationDecorator = new QuizEvaluationValidationDecorator(baseService);
    // Adăugăm feedback peste validare
    var feedbackDecorator = new QuizEvaluationFeedbackDecorator(validationDecorator);
    // Returnăm lanțul final de decoratori
    return feedbackDecorator;
});

// Înregistrăm factory-ul ca Singleton ca să păstreze obiectele partajate
builder.Services.AddSingleton<QuestionTypeFactory>();
// Înregistrăm serviciul care folosește Flyweight
builder.Services.AddScoped<QuestionRenderingService>();
// Repository pentru Question
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();

// Înregistrăm serviciul care aplică Bridge pentru afișarea lecțiilor
builder.Services.AddScoped<LessonBridgeService>();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configurăm Swagger doar în Development.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();