using BrightEdu.Application.DesignPatterns.AbstractFactory.Steps;
using BrightEdu.Application.DesignPatterns.Adapter.Steps;
using BrightEdu.Application.DesignPatterns.Builder;
using BrightEdu.Application.DesignPatterns.Facade.Steps;
using BrightEdu.Application.DesignPatterns.FactoryMethod.Steps;
using BrightEdu.Application.DesignPatterns.Singleton;
using BrightEdu.Application.Features.Courses;
using BrightEdu.Application.Features.Lessons;
using BrightEdu.Application.Features.Lessons.Mapper;
using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Infrastructure.DesignPatterns.AbstractFactory.Steps;
using BrightEdu.Infrastructure.DesignPatterns.Adapter.Steps;
using BrightEdu.Infrastructure.DesignPatterns.Builder;
using BrightEdu.Infrastructure.DesignPatterns.Composite.Steps;
using BrightEdu.Infrastructure.DesignPatterns.Facade.Steps;
using BrightEdu.Infrastructure.DesignPatterns.FactoryMethod.Steps;
using BrightEdu.Infrastructure.DesignPatterns.Singleton;
using BrightEdu.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

// aici leg contractul la implementare (DIP în practică)
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
builder.Services.AddScoped<ICreateLessonWithBuilderService, CreateLessonWithBuilderService>();
builder.Services.AddScoped<ICreateLessonWithPrototypeService, CreateLessonWithPrototypeService>();

// Builder și Director pentru construirea controlată a lecției.
builder.Services.AddScoped<ILessonBuilder, LessonBuilder>();
builder.Services.AddScoped<ILessonDirector, LessonDirector>();

// Builder și Director pentru construirea controlată a lecției.
builder.Services.AddSingleton<ILessonTemplateRegistry, LessonTemplateRegistry>();
builder.Services.AddScoped<ICreateLessonFromTemplateService, CreateLessonFromTemplateService>();

// Singleton pentru gestionarea template-urilor de lecții.
builder.Services.AddScoped<ILessonCreationAdapter, CreateLessonRequestAdapter>();
builder.Services.AddScoped<ICreateLessonWithAdapterService, CreateLessonWithAdapterService>();
builder.Services.AddScoped<ITemplateLessonAdapter, TemplateLessonAdapter>();
builder.Services.AddScoped<IGetTemplateAsCreationModelService, GetTemplateAsCreationModelService>();

// Servicii pentru patternul Adapter.
builder.Services.AddScoped<IBuildLessonCompositeService, BuildLessonCompositeService>();

// Serviciu pentru construirea structurii ierarhice cu Composite.
builder.Services.AddScoped<ILessonFacade, LessonFacade>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var templateRegistry = scope.ServiceProvider.GetRequiredService<ILessonTemplateRegistry>();

    var lesson1 = new Lesson(
        Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
        "Math Quiz Template");

    lesson1.AddStep(new ContentStep(
        Guid.NewGuid(),
        1,
        "Citește teoria despre fracții."));

    lesson1.AddStep(new QuestionStep(
        Guid.NewGuid(),
        2,
        "Care fracție este mai mare?",
        new List<string> { "1/4", "1/2", "1/8", "1/10" },
        1));

    var lesson2 = new Lesson(
        Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
        "Theory Template");

    lesson2.AddStep(new ContentStep(
        Guid.NewGuid(),
        1,
        "Introducere în lecție."));

    lesson2.AddStep(new ContentStep(
        Guid.NewGuid(),
        2,
        "Explicație suplimentară."));

    templateRegistry.AddTemplate(lesson1);
    templateRegistry.AddTemplate(lesson2);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();