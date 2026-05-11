using BrightEdu.Domain.Entities;

namespace BrightEdu.Application.Interfaces;

public interface IAdminLessonRepository
{
    Task<IReadOnlyList<Lesson>> GetByCourseIdAsync(Guid courseId, CancellationToken ct = default);
    Task<Lesson?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Lesson?> GetFullAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Lesson lesson, CancellationToken ct = default);
    Task RemoveAsync(Lesson lesson, CancellationToken ct = default);
    Task SaveAsync(CancellationToken ct = default);
    Task DeleteContentBlocksAsync(Guid lessonId, CancellationToken ct = default);
    void AddContentBlock(LessonContentBlock block);
    void AddAttachment(LessonAttachment attachment);
    Task DeleteAttachmentAsync(Guid attachmentId, CancellationToken ct = default);
}
