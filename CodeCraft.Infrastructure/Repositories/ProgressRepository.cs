using CodeCraft.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Infrastructure.Repositories;
public class ProgressRepository: IProgressRepository
{
    private readonly AppDbContext _context;

    public ProgressRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Lesson?> GetLastLessonAsync(string userId)
    {
        return await _context.UserLessonProgresses
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.LastWatchedAt)
            .Select(x => x.Lesson)
            .FirstOrDefaultAsync();
    }

    public async Task<double> GetCourseProgressAsync(int userId, int courseId)
    {
        var totalLessons = await _context.Lessons
            .CountAsync(l => l.CourseId == courseId);

        if (totalLessons == 0)
            return 0;

        var completedLessons = await _context.UserLessonProgresses
            .CountAsync(p =>
                p.Id == userId &&
                p.IsCompleted &&
                p.Lesson.CourseId == courseId
            );

        return (double)completedLessons / totalLessons * 100;
    }

    public async Task<UserLessonProgress?> GetByUserAndLesson(int userId, int lessonId)
    {
        return await _context.UserLessonProgresses
    .FirstOrDefaultAsync(p => p.UserId == userId.ToString() && p.LessonId == lessonId);
    }
}
