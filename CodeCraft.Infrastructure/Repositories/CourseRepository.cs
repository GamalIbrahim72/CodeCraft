using CodeCraft.Application.Interfaces.Repositories;
using CodeCraft.Domain.Entities;
using CodeCraft.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CodeCraft.Infrastructure.Repositories;

public class CourseRepository : GenericRepository<Course>, ICourseRepository
{
    public CourseRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Course>> GetCoursesByTrackId(int trackId)
    {
        return await _context.Courses
            .Where(x => x.TrackId == trackId)
            .ToListAsync();
    }

    public async Task<int> GetNextOrder(int trackId)
    {
        var lastCourse = await _context.Courses
            .Where(c => c.TrackId == trackId)
            .OrderByDescending(c => c.Order)
            .FirstOrDefaultAsync();

        return lastCourse == null ? 1 : lastCourse.Order + 1;
    }
}
