using CodeCraft.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Infrastructure.Repositories;
public class CourseRepository: GenericRepository<Course>, ICourseRepository
{
    private readonly AppDbContext _context;

    public CourseRepository(AppDbContext context) : base(context)
    {
        _context = context;
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
