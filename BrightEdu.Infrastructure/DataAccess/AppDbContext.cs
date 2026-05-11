using BrightEdu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BrightEdu.Infrastructure.DataAccess;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseTranslation> CourseTranslations => Set<CourseTranslation>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<ModuleTranslation> ModuleTranslations => Set<ModuleTranslation>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<LessonTranslation> LessonTranslations => Set<LessonTranslation>();
    public DbSet<LessonContentBlock> LessonContentBlocks => Set<LessonContentBlock>();
    public DbSet<LessonAttachment> LessonAttachments => Set<LessonAttachment>();
    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionTranslation> QuestionTranslations => Set<QuestionTranslation>();
    public DbSet<Answer> Answers => Set<Answer>();
    public DbSet<AnswerOptionTranslation> AnswerOptionTranslations => Set<AnswerOptionTranslation>();
    public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();
    public DbSet<QuizAttemptAnswer> QuizAttemptAnswers => Set<QuizAttemptAnswer>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<LessonProgress> LessonProgresses => Set<LessonProgress>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CourseCategory> CourseCategories => Set<CourseCategory>();
    public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(256);
            entity.Property(x => x.PasswordHash).IsRequired().HasMaxLength(512);
            entity.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(x => x.LastName).IsRequired().HasMaxLength(100);
            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(50);
            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(x => new { x.UserId, x.RoleId });
            entity.HasOne(x => x.User).WithMany("UserRoles").HasForeignKey(x => x.UserId);
            entity.HasOne(x => x.Role).WithMany("UserRoles").HasForeignKey(x => x.RoleId);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Token).IsRequired().HasMaxLength(512);
            entity.HasOne(x => x.User).WithMany("RefreshTokens").HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Slug).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Level).IsRequired().HasMaxLength(50);
            entity.HasIndex(x => x.Slug).IsUnique();
            entity.HasOne(x => x.ThumbnailMediaAsset).WithMany().HasForeignKey(x => x.ThumbnailMediaAssetId).OnDelete(DeleteBehavior.SetNull);
            entity.HasMany(x => x.Modules).WithOne(x => x.Course).HasForeignKey(x => x.CourseId);
            entity.HasMany(x => x.Lessons).WithOne(x => x.Course).HasForeignKey(x => x.CourseId);
            entity.HasMany(x => x.Translations).WithOne(x => x.Course).HasForeignKey(x => x.CourseId);
            entity.HasMany(x => x.CourseCategories).WithOne(x => x.Course).HasForeignKey(x => x.CourseId);
            entity.HasMany(x => x.Enrollments).WithOne(x => x.Course).HasForeignKey(x => x.CourseId);
        });

        modelBuilder.Entity<CourseTranslation>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).IsRequired().HasMaxLength(200);
            entity.Property(x => x.ShortDescription).HasMaxLength(1000);
            entity.Property(x => x.FullDescription).HasMaxLength(4000);
            entity.HasIndex(x => new { x.CourseId, x.LanguageCode }).IsUnique();
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasMany(x => x.Translations).WithOne(x => x.Module).HasForeignKey(x => x.ModuleId);
            entity.HasMany(x => x.Lessons).WithOne(x => x.Module).HasForeignKey(x => x.ModuleId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.Quiz).WithOne(x => x.Module).HasForeignKey<Quiz>(x => x.ModuleId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<ModuleTranslation>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Description).HasMaxLength(1000);
            entity.HasIndex(x => new { x.ModuleId, x.LanguageCode }).IsUnique();
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.EstimatedMinutes).IsRequired();
            entity.HasMany(x => x.Translations).WithOne(x => x.Lesson).HasForeignKey(x => x.LessonId);
            entity.HasMany(x => x.ContentBlocks).WithOne(x => x.Lesson).HasForeignKey(x => x.LessonId);
            entity.HasMany(x => x.Attachments).WithOne(x => x.Lesson).HasForeignKey(x => x.LessonId);
            entity.HasMany(x => x.ProgressEntries).WithOne(x => x.Lesson).HasForeignKey(x => x.LessonId);
            entity.HasOne(x => x.Quiz).WithOne(x => x.Lesson).HasForeignKey<Quiz>(x => x.LessonId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<LessonTranslation>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Summary).IsRequired().HasMaxLength(4000);
            entity.HasIndex(x => new { x.LessonId, x.LanguageCode }).IsUnique();
        });

        modelBuilder.Entity<LessonContentBlock>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ConfigJson).IsRequired().HasMaxLength(8000);
            entity.Property(x => x.Lang).IsRequired().HasMaxLength(10).HasDefaultValue("ro");
        });

        modelBuilder.Entity<LessonAttachment>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.DisplayName).IsRequired().HasMaxLength(200);
            entity.Property(x => x.MediaAssetId).IsRequired(false);
            entity.Property(x => x.ExternalUrl).IsRequired(false).HasMaxLength(1000);
            entity.HasOne(x => x.MediaAsset).WithMany().HasForeignKey(x => x.MediaAssetId).IsRequired(false);
        });

        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).IsRequired().HasMaxLength(200);
            entity.Property(x => x.LessonId).IsRequired(false);
            entity.Property(x => x.ModuleId).IsRequired(false);
            entity.HasMany(x => x.Questions).WithOne(x => x.Quiz).HasForeignKey(x => x.QuizId);
            entity.HasMany(x => x.Attempts).WithOne(x => x.Quiz).HasForeignKey(x => x.QuizId);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ConfigJson).IsRequired().HasMaxLength(4000);
            entity.HasMany(x => x.Answers).WithOne(x => x.Question).HasForeignKey(x => x.QuestionId);
            entity.HasMany(x => x.Translations).WithOne(x => x.Question).HasForeignKey(x => x.QuestionId);
        });

        modelBuilder.Entity<QuestionTranslation>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Text).IsRequired().HasMaxLength(1000);
            entity.Property(x => x.Explanation).HasMaxLength(2000);
            entity.HasIndex(x => new { x.QuestionId, x.LanguageCode }).IsUnique();
        });

        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasMany(x => x.Translations).WithOne(x => x.Answer).HasForeignKey(x => x.AnswerId);
        });

        modelBuilder.Entity<AnswerOptionTranslation>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Text).IsRequired().HasMaxLength(500);
            entity.HasIndex(x => new { x.AnswerId, x.LanguageCode }).IsUnique();
        });

        modelBuilder.Entity<QuizAttempt>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Score).HasPrecision(5, 2);
            entity.HasOne(x => x.Student).WithMany("QuizAttempts").HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(x => x.Answers).WithOne(x => x.QuizAttempt).HasForeignKey(x => x.QuizAttemptId);
        });

        modelBuilder.Entity<QuizAttemptAnswer>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TextAnswer).HasMaxLength(4000);
            entity.HasOne(x => x.SelectedOption).WithMany().HasForeignKey(x => x.SelectedOptionId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.Question).WithMany().HasForeignKey(x => x.QuestionId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasOne(x => x.Student).WithMany("Enrollments").HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.StudentId, x.CourseId }).IsUnique();
        });

        modelBuilder.Entity<LessonProgress>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasOne(x => x.Student).WithMany("LessonProgresses").HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.StudentId, x.LessonId }).IsUnique();
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<CourseCategory>(entity =>
        {
            entity.HasKey(x => new { x.CourseId, x.CategoryId });
            entity.HasOne(x => x.Category).WithMany("CourseCategories").HasForeignKey(x => x.CategoryId);
        });

        modelBuilder.Entity<MediaAsset>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FileName).IsRequired().HasMaxLength(260);
            entity.Property(x => x.StoredFileName).IsRequired().HasMaxLength(260);
            entity.Property(x => x.RelativePath).IsRequired().HasMaxLength(500);
            entity.Property(x => x.MimeType).IsRequired().HasMaxLength(150);
        });
    }
}
