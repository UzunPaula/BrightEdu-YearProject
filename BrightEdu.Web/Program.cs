using System.Text;
using BrightEdu.Application.DesignPatterns.Bridge;
using BrightEdu.Application.DesignPatterns.Decorator;
using BrightEdu.Application.DesignPatterns.Flyweight;
using BrightEdu.Application.DesignPatterns.ChainOfResponsibility;
using BrightEdu.Application.DesignPatterns.Mediator;
using BrightEdu.Application.DesignPatterns.Observer;
using BrightEdu.Application.DesignPatterns.Proxy;
using BrightEdu.Application.DesignPatterns.Strategy;
using BrightEdu.Application.DesignPatterns.TemplateMethod;
using BrightEdu.Application.DesignPatterns.Visitor;
using BrightEdu.Application.Features.Admin;
using BrightEdu.Application.Features.Auth;
using BrightEdu.Application.Features.Catalog;
using BrightEdu.Application.Features.Courses;
using BrightEdu.Application.Features.Lessons;
using BrightEdu.Application.Features.Quizzes;
using BrightEdu.Application.Features.QuizAttempts;
using BrightEdu.Application.Features.Students;
using BrightEdu.Application.Interfaces;
using BrightEdu.Infrastructure.DataAccess;
using BrightEdu.Infrastructure.Repositories;
using BrightEdu.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introdu token-ul JWT: Bearer {token}"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("BrightEduFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

var jwtSection = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSection["SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey lipsește.");
var issuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer lipsește.");
var audience = jwtSection["Audience"] ?? throw new InvalidOperationException("Jwt:Audience lipsește.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<ICourseReadRepository, CourseRepository>();
builder.Services.AddScoped<ICourseWriteRepository, CourseRepository>();
builder.Services.AddScoped<ICourseCatalogRepository, CourseCatalogRepository>();
builder.Services.AddScoped<ILessonRepository, LessonRepository>();
builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IQuizAttemptRepository, QuizAttemptRepository>();
builder.Services.AddScoped<ILessonProgressRepository, LessonProgressRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IPasswordHashService, Pbkdf2PasswordHashService>();
builder.Services.AddScoped<IAuthTokenService, JwtTokenService>();
builder.Services.AddScoped<Pbkdf2PasswordHashService>();
builder.Services.AddScoped<SampleDataSeeder>();

builder.Services.AddScoped<IGetCoursesService, GetCoursesService>();
builder.Services.AddScoped<IPublicCourseCatalogService, PublicCourseCatalogService>();
builder.Services.AddScoped<IGetLessonService, GetLessonService>();
builder.Services.AddScoped<IPublicLessonService, PublicLessonService>();
builder.Services.AddScoped<ICreateLessonService, CreateLessonService>();
builder.Services.AddScoped<ICreateQuizService, CreateQuizService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStartQuizAttemptService, StartQuizAttemptService>();
builder.Services.AddScoped<ISubmitQuizAttemptService, SubmitQuizAttemptService>();
builder.Services.AddScoped<IQuizAttemptHistoryService, QuizAttemptHistoryService>();
builder.Services.AddScoped<ICourseEnrollmentService, CourseEnrollmentService>();
builder.Services.AddScoped<ILessonProgressService, LessonProgressService>();
builder.Services.AddScoped<IStudentDashboardService, StudentDashboardService>();

builder.Services.AddScoped<IAdminCourseRepository, AdminCourseRepository>();
builder.Services.AddScoped<IAdminModuleRepository, AdminModuleRepository>();
builder.Services.AddScoped<IAdminLessonRepository, AdminLessonRepository>();
builder.Services.AddScoped<IAdminQuizRepository, AdminQuizRepository>();
builder.Services.AddScoped<IAdminQuestionRepository, AdminQuestionRepository>();
builder.Services.AddScoped<IAdminCourseService, AdminCourseService>();
builder.Services.AddScoped<IAdminCourseImportService, AdminCourseImportService>();
builder.Services.AddScoped<IAdminModuleService, AdminModuleService>();
builder.Services.AddScoped<IAdminLessonService, AdminLessonService>();
builder.Services.AddScoped<IAdminQuizService, AdminQuizService>();
builder.Services.AddScoped<IAdminQuestionService, AdminQuestionService>();
builder.Services.AddScoped<IMediaAssetRepository, MediaAssetRepository>();
builder.Services.AddScoped<IFileStorageService>(_ =>
{
    var uploadsRoot = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads");
    return new LocalFileStorageService(uploadsRoot);
});
builder.Services.AddScoped<IAdminMediaService, AdminMediaService>();

builder.Services.AddScoped<QuizAccessService>();
builder.Services.AddScoped<IQuizAccessService, QuizAccessProxy>();
builder.Services.AddScoped<QuizEvaluationService>();
builder.Services.AddScoped<IQuizEvaluationService>(sp =>
{
    var baseService = sp.GetRequiredService<QuizEvaluationService>();
    var validationDecorator = new QuizEvaluationValidationDecorator(baseService);
    var feedbackDecorator = new QuizEvaluationFeedbackDecorator(validationDecorator);
    return feedbackDecorator;
});

builder.Services.AddSingleton<QuestionTypeFactory>();
builder.Services.AddScoped<QuestionRenderingService>();
builder.Services.AddScoped<LessonBridgeService>();

// Lab6 — Behavioral Patterns
// Observer
builder.Services.AddScoped<IQuizResultObserver, LessonAutoCompleteObserver>();
// Strategy — scor uniform implicit; poate fi schimbat în WeightedScoringStrategy
builder.Services.AddScoped<IQuizScoringStrategy, UniformScoringStrategy>();
builder.Services.AddScoped<QuizScoringContext>();
// Template Method
builder.Services.AddScoped<EnrolledStudentLessonAccess>();
builder.Services.AddScoped<AdminLessonAccess>();
// Chain of Responsibility — handlere instanțiate per request în StartQuizAttemptService
// Mediator
builder.Services.AddScoped<ILessonCompletionMediator>(sp =>
{
    var mediator = new LessonCompletionMediator();
    var progressColleague = new CourseProgressColleague(
        sp.GetRequiredService<ILessonProgressRepository>(),
        sp.GetRequiredService<ILessonRepository>(),
        sp.GetRequiredService<IEnrollmentRepository>());
    mediator.Register(progressColleague);
    return mediator;
});
// Visitor — instanțiate ad-hoc unde este nevoie (ContentBlockValidationVisitor, ContentBlockStatisticsVisitor)

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<SampleDataSeeder>();
    await seeder.SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("BrightEduFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
