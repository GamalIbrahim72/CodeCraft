using CodeCraft.Application.DTOs.Progress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.Interfaces.Services;
public interface IProgressService
{
    Task MarkLessonComplete(int lessonId, string userId);
    Task<int> GetCourseProgress(int courseId, string userId);
    Task UpdateWatchTime(UpdateWatchTimeRequest request, string userId);

    Task<int> GetWatchTime(int lessonId, string userId);
    Task<int> GetTrackProgress(int trackId, string userId);

    Task<Lesson?> GetContinueLearningAsync(string userId);
    Task StartCourse(int userId, int courseId);
}
