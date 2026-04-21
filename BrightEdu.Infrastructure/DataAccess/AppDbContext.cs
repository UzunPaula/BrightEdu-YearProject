using BrightEdu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BrightEdu.Infrastructure.DataAccess;

// Clasa principală EF Core care face legătura dintre aplicație și baza de date.
// Aici declarăm tabelele și configurăm relațiile dintre entități.
public class AppDbContext : DbContext
{
    // Constructorul primește opțiunile de configurare din Program.cs.
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Fiecare DbSet corespunde unui tabel din baza de date.
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Answer> Answers => Set<Answer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurare pentru Course
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(c => c.Description)
                .HasMaxLength(1000);

            // Un Course are multe Lesson.
            entity.HasMany(c => c.Lessons)
                .WithOne(l => l.Course)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configurare pentru Lesson
        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(l => l.Id);

            entity.Property(l => l.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(l => l.Content)
                .IsRequired()
                .HasMaxLength(4000);

            entity.Property(l => l.Order)
                .IsRequired();

            // O Lesson poate avea un singur Quiz.
            entity.HasOne(l => l.Quiz)
                .WithOne(q => q.Lesson)
                .HasForeignKey<Quiz>(q => q.LessonId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configurare pentru Quiz
        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.HasKey(q => q.Id);

            entity.Property(q => q.Title)
                .IsRequired()
                .HasMaxLength(200);

            // Un Quiz are multe Question.
            entity.HasMany(q => q.Questions)
                .WithOne(qst => qst.Quiz)
                .HasForeignKey(qst => qst.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configurare pentru Question
        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(q => q.Id);

            entity.Property(q => q.Text)
                .IsRequired()
                .HasMaxLength(500);

            // O Question are multe Answer.
            entity.HasMany(q => q.Answers)
                .WithOne(a => a.Question)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configurare pentru Answer
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Text)
                .IsRequired()
                .HasMaxLength(300);

            entity.Property(a => a.IsCorrect)
                .IsRequired();
        });
    }
}