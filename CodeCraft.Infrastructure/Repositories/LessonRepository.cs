using CodeCraft.Application.Interfaces.Repositories;
using CodeCraft.Domain.Entities;
using CodeCraft.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CodeCraft.Infrastructure.Repositories;

public class LessonRepository : GenericRepository<Lesson>, ILessonRepository
{
    public LessonRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<int> GetLessonsCountByCourseId(int courseId)
    {
        return await _context.Lessons
            .Where(x => x.CourseId == courseId)
            .CountAsync();
    }

    public async Task<Lesson?> GetFirstLessonInCourse(int courseId)
    {
        return await _context.Lessons
            .Where(l => l.CourseId == courseId)
            .OrderBy(l => l.Order)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Lesson>> GetLessonsByCourseId(int courseId)
    {
        return await _context.Lessons
            .Where(l => l.CourseId == courseId)
            .ToListAsync();
    }
}
