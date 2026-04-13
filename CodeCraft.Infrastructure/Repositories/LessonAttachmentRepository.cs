using CodeCraft.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Infrastructure.Repositories;
public class LessonAttachmentRepository:GenericRepository<LessonAttachment> ,ILessonAttachmentRepository
{
    private readonly AppDbContext _context;

    public LessonAttachmentRepository(AppDbContext context):base(context)
    {
        _context = context;
    }

    public async Task AddAsync(LessonAttachment attachment)
    {
        await _context.LessonAttachments.AddAsync(attachment);
    }

    public async Task<IEnumerable<LessonAttachment>> GetByLessonIdAsync(int lessonId)
    {
        return await _context.LessonAttachments
            .Where(a => a.LessonId == lessonId)
            .ToListAsync();
    }

    public async Task<LessonAttachment> GetByIdAsync(int id)
    {
        return await _context.LessonAttachments.FindAsync(id);
    }

    public void Delete(LessonAttachment attachment)
    {
        _context.LessonAttachments.Remove(attachment);
    }
}
