using BrightEdu.Application.Features.Courses;
using BrightEdu.Application.Features.Lessons;
using BrightEdu.Application.Features.Lessons.Mapper;
using BrightEdu.Application.Interfaces;
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
// Mappers
builder.Services.AddScoped<ILessonStepDtoMapper, ContentStepDtoMapper>();
builder.Services.AddScoped<ILessonStepDtoMapper, QuestionStepDtoMapper>();

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