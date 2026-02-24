using BrightEdu.Application.DesignPatterns.AbstractFactory.Steps;
using BrightEdu.Application.DesignPatterns.FactoryMethod.Steps;
using BrightEdu.Application.Features.Courses;
using BrightEdu.Application.Features.Lessons;
using BrightEdu.Application.Features.Lessons.Mapper;
using BrightEdu.Application.Interfaces;
using BrightEdu.Infrastructure.DesignPatterns.AbstractFactory.Steps;
using BrightEdu.Infrastructure.DesignPatterns.FactoryMethod.Steps;
using BrightEdu.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

// aici leg contractul la implementare (DIP(Dependency Inversion Principle) în practică)
// Repositories
builder.Services.AddScoped<ICourseReadRepository, InMemoryCourseRepository>();
builder.Services.AddScoped<ICourseWriteRepository, InMemoryCourseRepository>();
builder.Services.AddScoped<ILessonRepository, InMemoryLessonRepository>();

// Services (use cases)
builder.Services.AddScoped<IGetCoursesService, GetCoursesService>();
builder.Services.AddScoped<IGetLessonService, GetLessonService>();

// OCP: înregistrez mapper-ele, iar service-ul le folosește automat.
// Mappers pentru DTO
builder.Services.AddScoped<ContentStepDtoMapper>();
builder.Services.AddScoped<QuestionStepDtoMapper>();
builder.Services.AddScoped<ILessonStepDtoMapper, ContentStepDtoMapper>();
builder.Services.AddScoped<ILessonStepDtoMapper, QuestionStepDtoMapper>();

// Factory Method: înregistrez creatorii concreți pentru fiecare tip de step.
// Extind aplicația adăugând un nou creator, fără să modific mapperul sau service-ul.
builder.Services.AddScoped<ContentStepCreator>();
builder.Services.AddScoped<QuestionStepCreator>();

// Factory Method: resolverul alege creatorul potrivit după dto.Type, ex: "content", "question".
builder.Services.AddScoped<ILessonStepCreator, ContentStepCreator>();
builder.Services.AddScoped<ILessonStepCreator, QuestionStepCreator>();
builder.Services.AddScoped<ILessonStepCreatorResolver, LessonStepCreatorResolver>();

// Abstract Factory
builder.Services.AddScoped<ILessonStepAbstractFactory, ContentLessonStepFactory>();
builder.Services.AddScoped<ILessonStepAbstractFactory, QuestionLessonStepFactory>();
builder.Services.AddScoped<ILessonStepAbstractFactoryResolver, LessonStepAbstractFactoryResolver>();

// Leagă interfața de implementare și creează o instanță nouă pe fiecare request.
//builder.Services.AddScoped<ICreateLessonService, CreateLessonService>();

builder.Services.AddScoped<ICreateLessonWithFactoryMethodService, CreateLessonWithFactoryMethodService>();
builder.Services.AddScoped<ICreateLessonWithAbstractFactoryService, CreateLessonWithAbstractFactoryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();