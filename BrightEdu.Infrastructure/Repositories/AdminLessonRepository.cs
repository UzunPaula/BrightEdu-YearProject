using BrightEdu.Application.Interfaces;
using BrightEdu.Domain.Entities;
using BrightEdu.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace BrightEdu.Infrastructure.Repositories;

public sealed class AdminLessonRepository : IAdminLessonRepository
{
    private readonly AppDbContext _dbContext;

    public AdminLessonRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Lesson>> GetByCourseIdAsync(Guid courseId, CancellationToken ct = default)
    {
        return await _dbContext.Lessons
            .Include(x => x.Translations)
            .Include(x => x.Quiz)
            .Where(x => x.CourseId == courseId)
            .OrderBy(x => x.Order)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Lesson?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.Lessons
            .Include(x => x.Translations)
            .Include(x => x.Quiz)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<Lesson?> GetFullAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.Lessons
            .Include(x => x.Translations)
            .Include(x => x.Quiz)
            .Include(x => x.ContentBlocks)
            .Include(x => x.Attachments)
                .ThenInclude(a => a.MediaAsset)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task AddAsync(Lesson lesson, CancellationToken ct = default)
    {
        await _dbContext.Lessons.AddAsync(lesson, ct);
    }

    public Task RemoveAsync(Lesson lesson, CancellationToken ct = default)
    {
        _dbContext.Lessons.Remove(lesson);
        return Task.CompletedTask;
    }

    public async Task SaveAsync(CancellationToken ct = default)
    {
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteContentBlocksAsync(Guid lessonId, CancellationToken ct = default)
    {
        var blocks = await _dbContext.Set<LessonContentBlock>()
            .Where(b => b.LessonId == lessonId)
            .ToListAsync(ct);
        _dbContext.Set<LessonContentBlock>().RemoveRange(blocks);
    }

    public void AddContentBlock(LessonContentBlock block)
    {
        _dbContext.Set<LessonContentBlock>().Add(block);
    }

    public void AddAttachment(LessonAttachment attachment)
    {
        _dbContext.Set<LessonAttachment>().Add(attachment);
    }

    public async Task DeleteAttachmentAsync(Guid attachmentId, CancellationToken ct = default)
    {
        var attachment = await _dbContext.Set<LessonAttachment>()
            .FirstOrDefaultAsync(a => a.Id == attachmentId, ct);
        if (attachment is not null)
            _dbContext.Set<LessonAttachment>().Remove(attachment);
    }
}
