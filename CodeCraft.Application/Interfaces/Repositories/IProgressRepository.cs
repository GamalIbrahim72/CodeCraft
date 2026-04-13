using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.Interfaces.Repositories;
public interface IProgressRepository
{
    Task<Lesson?> GetLastLessonAsync(string userId);
    Task<double> GetCourseProgressAsync(int userId, int courseId);
    Task<UserLessonProgress?> GetByUserAndLesson(int userId, int lessonId);
}
