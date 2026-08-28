using CodeCraft.Application.Interfaces.Repositories;
using CodeCraft.Domain.Entities;
using CodeCraft.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CodeCraft.Infrastructure.Repositories;

public class LessonAttachmentRepository : GenericRepository<LessonAttachment>, ILessonAttachmentRepository
{
    public LessonAttachmentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<LessonAttachment>> GetByLessonIdAsync(int lessonId)
    {
        return await _context.LessonAttachments
            .Where(a => a.LessonId == lessonId)
            .ToListAsync();
    }

    public void Delete(LessonAttachment attachment)
    {
        _context.LessonAttachments.Remove(attachment);
    }
}
