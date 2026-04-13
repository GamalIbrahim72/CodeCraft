using CodeCraft.Application.DTOs.Progress;
using CodeCraft.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.Services;
public class ProgressService: IProgressService
{
    private readonly IGenericRepository<UserLessonProgress> _progressRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IProgressRepository _progressRepo;

    public ProgressService(
        IGenericRepository<UserLessonProgress> progressRepository,
        ILessonRepository lessonRepository,
        ICourseRepository courseRepository,
        IProgressRepository progressRepo)
    {
        _progressRepository = progressRepository;
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
        _progressRepo = progressRepo;
    }

    // ✅ Mark Lesson Complete (Fix duplicate problem)
    public async Task MarkLessonComplete(int lessonId, string userId)
    {
        var existing = (await _progressRepository.FindAsync(x =>
            x.UserId == userId && x.LessonId == lessonId))
            .FirstOrDefault();

        if (existing == null)
        {
            var progress = new UserLessonProgress
            {
                LessonId = lessonId,
                UserId = userId,
                IsCompleted = true,
                LastWatchedAt = DateTime.UtcNow
            };

            await _progressRepository.AddAsync(progress);
        }
        else
        {
            existing.IsCompleted = true;
            existing.LastWatchedAt = DateTime.UtcNow;

            await _progressRepository.UpdateAsync(existing);
        }
    }

    // ✅ Course Progress %
    public async Task<int> GetCourseProgress(int courseId, string userId)
    {
        var totalLessons = await _lessonRepository.GetLessonsCountByCourseId(courseId);

        if (totalLessons == 0)
            return 0;

        var progresses = await _progressRepository.FindAsync(x =>
            x.UserId == userId && x.IsCompleted);

        var completedLessons = progresses
            .Where(x => x.Lesson != null && x.Lesson.CourseId == courseId)
            .Count();

        return (completedLessons * 100) / totalLessons;
    }

    // ✅ Watch Time (Update or Insert)
    public async Task UpdateWatchTime(UpdateWatchTimeRequest request, string userId)
    {
        var progress = (await _progressRepository.FindAsync(x =>
            x.UserId == userId && x.LessonId == request.LessonId))
            .FirstOrDefault();

        if (progress == null)
        {
            progress = new UserLessonProgress
            {
                LessonId = request.LessonId,
                UserId = userId,
                WatchedSeconds = request.WatchedSeconds,
                LastWatchedAt = DateTime.UtcNow
            };

            await _progressRepository.AddAsync(progress);
        }
        else
        {
            progress.WatchedSeconds = request.WatchedSeconds;
            progress.LastWatchedAt = DateTime.UtcNow;

            await _progressRepository.UpdateAsync(progress);
        }
    }

    // ✅ Get Watch Time
    public async Task<int> GetWatchTime(int lessonId, string userId)
    {
        var progress = (await _progressRepository.FindAsync(x =>
            x.UserId == userId && x.LessonId == lessonId))
            .FirstOrDefault();

        return progress?.WatchedSeconds ?? 0;
    }

    // ✅ Track Progress (Average of Courses)
    public async Task<int> GetTrackProgress(int trackId, string userId)
    {
        var courses = await _courseRepository.GetCoursesByTrackId(trackId);

        if (!courses.Any())
            return 0;

        int totalProgress = 0;

        foreach (var course in courses)
        {
            var courseProgress = await GetCourseProgress(course.Id, userId);
            totalProgress += courseProgress;
        }

        return totalProgress / courses.Count();
    }

    // ✅ Continue Learning
    public async Task<Lesson?> GetContinueLearningAsync(string userId)
    {
        return await _progressRepo.GetLastLessonAsync(userId);
    }

    public async Task StartCourse(int userId, int courseId)
    {
        // 1️⃣ هات أول lesson في الكورس
        var firstLesson = await _lessonRepository.GetFirstLessonInCourse(courseId);

        if (firstLesson == null)
            return;

        // 2️⃣ شوف هل فيه progress قبل كدة
        var existing = await _progressRepo
            .GetByUserAndLesson(userId, firstLesson.Id);

        if (existing != null)
            return;

        // 3️⃣ سجل بداية الكورس
        var progress = new UserLessonProgress
        {
            UserId = userId.ToString(),
            LessonId = firstLesson.Id,
            IsCompleted = false,
            WatchedSeconds = 0
        };

        await _progressRepository.AddAsync(progress);
    }


}
