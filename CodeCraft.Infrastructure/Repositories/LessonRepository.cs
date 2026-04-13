using CodeCraft.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Infrastructure.Repositories;
public class LessonRepository:GenericRepository<Lesson>, ILessonRepository
{
    private readonly AppDbContext _context;

    public LessonRepository(AppDbContext context) : base(context)
    {
        _context = context;
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
}
