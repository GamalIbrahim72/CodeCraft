using CodeCraft.Domain.Entities;

namespace CodeCraft.Application.Interfaces.Repositories;

public interface ILessonAttachmentRepository : IGenericRepository<LessonAttachment>
{
    Task<IEnumerable<LessonAttachment>> GetByLessonIdAsync(int lessonId);
    void Delete(LessonAttachment attachment);
}
