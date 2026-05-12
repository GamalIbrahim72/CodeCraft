using CodeCraft.Application.DTOs.Progress;
using CodeCraft.Application.Interfaces.Repositories;

namespace CodeCraft.Application.Services;

public class ProgressService : IProgressService
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

    public async Task<int> GetCourseProgress(int courseId, string userId)
    {
        var lessons = await _lessonRepository.GetLessonsByCourseId(courseId);

        var lessonIds = lessons
            .Select(l => l.Id)
            .ToList();

        var totalLessons = lessonIds.Count;

        if (totalLessons == 0)
            return 0;

        var progresses = await _progressRepository.FindAsync(x =>
            x.UserId == userId &&
            x.IsCompleted &&
            lessonIds.Contains(x.LessonId));

        var completedLessons = progresses.Count();

        return (completedLessons * 100) / totalLessons;
    }

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

    public async Task<int> GetWatchTime(int lessonId, string userId)
    {
        var progress = (await _progressRepository.FindAsync(x =>
            x.UserId == userId && x.LessonId == lessonId))
            .FirstOrDefault();

        return progress?.WatchedSeconds ?? 0;
    }

    public async Task<int> GetTrackProgress(int trackId, string userId)
    {
        var courses = await _courseRepository.GetCoursesByTrackId(trackId);

        var courseList = courses.ToList();

        if (!courseList.Any())
            return 0;

        int totalProgress = 0;

        foreach (var course in courseList)
        {
            var courseProgress = await GetCourseProgress(course.Id, userId);
            totalProgress += courseProgress;
        }

        return totalProgress / courseList.Count;
    }

    public async Task<Lesson?> GetContinueLearningAsync(string userId)
    {
        return await _progressRepo.GetLastLessonAsync(userId);
    }

    public async Task StartCourse(int userId, int courseId)
    {
        var firstLesson = await _lessonRepository.GetFirstLessonInCourse(courseId);

        if (firstLesson == null)
            return;

        var existing = await _progressRepo.GetByUserAndLesson(userId, firstLesson.Id);

        if (existing != null)
            return;

        var progress = new UserLessonProgress
        {
            UserId = userId.ToString(),
            LessonId = firstLesson.Id,
            IsCompleted = false,
            WatchedSeconds = 0,
            LastWatchedAt = DateTime.UtcNow
        };

        await _progressRepository.AddAsync(progress);
    }
}