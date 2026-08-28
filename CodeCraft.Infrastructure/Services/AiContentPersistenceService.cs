using CodeCraft.Application.DTOs.AI;
using CodeCraft.Domain.Entities;
using CodeCraft.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CodeCraft.Infrastructure.Services;

public class AiContentPersistenceService : IAiContentPersistenceService
{
    private readonly AppDbContext _context;
    private readonly ILogger<AiContentPersistenceService> _logger;

    public AiContentPersistenceService(
        AppDbContext context,
        ILogger<AiContentPersistenceService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SaveGeneratedContentAsync(AiRoadmapResponseDto data)
    {
        if (data.Data == null)
            throw new ArgumentNullException(nameof(data), "AI roadmap data is null");

        var aiTrackName = NormalizeTrackName(data.Data.Track);

        var track = await _context.Tracks
            .FirstOrDefaultAsync(t =>
                t.Name.ToLower() == aiTrackName ||
                t.Name.ToLower().Replace(" ", "_") == aiTrackName ||
                t.Name.ToLower().Replace(" ", "") == aiTrackName.Replace("_", ""));

        if (track == null)
            throw new InvalidOperationException($"Track not found: {data.Data.Track}");

        var level = NormalizeLevel(data.Data.Level);

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            foreach (var step in data.Data.Roadmap)
            {
                var courseTitle =
                    step.MainTopic
                    ?? step.Topic
                    ?? step.TopicName
                    ?? step.TopicId
                    ?? $"Step {step.Step}";

                var externalTopicId = step.TopicId ?? $"step-{step.Step}";

                var course = await _context.Courses.FirstOrDefaultAsync(c =>
                    c.TrackId == track.Id &&
                    c.Level.ToLower() == level.ToLower() &&
                    c.ExternalTopicId == externalTopicId);

                if (course == null)
                {
                    course = new Course
                    {
                        TrackId = track.Id,
                        Level = level,
                        ExternalTopicId = externalTopicId
                    };

                    _context.Courses.Add(course);
                }

                course.Level = level;
                course.Title = $"{courseTitle} - {level}";
                course.Description = $"AI Generated Course - Topic Id: {externalTopicId}";
                course.Order = step.Step;

                await _context.SaveChangesAsync();

                var lessonOrder = 1;

                foreach (var lessonDto in step.Lessons)
                {
                    var videos = lessonDto.Resources?.Videos;

                    var externalLessonId =
                        lessonDto.LessonId
                        ?? $"{externalTopicId}-lesson-{lessonOrder}";

                    var lesson = await _context.Lessons.FirstOrDefaultAsync(l =>
                        l.CourseId == course.Id &&
                        l.ExternalLessonId == externalLessonId);

                    if (lesson == null)
                    {
                        lesson = new Lesson
                        {
                            CourseId = course.Id,
                            ExternalLessonId = externalLessonId
                        };

                        _context.Lessons.Add(lesson);
                    }

                    lesson.Title = lessonDto.Subtopic ?? lessonDto.Topic ?? $"Lesson {lessonOrder}";
                    lesson.Description = lessonDto.Description ?? string.Empty;

                    lesson.VideoUrl = GetVideo(videos, "video_1");
                    lesson.VideoUrl2 = GetVideo(videos, "video_2");
                    lesson.VideoUrl3 = GetVideo(videos, "video_3");

                    lesson.ArticleUrl = lessonDto.Resources?.Article ?? string.Empty;
                    lesson.Order = lessonOrder;

                    lessonOrder++;
                }

                await _context.SaveChangesAsync();
            }

            await transaction.CommitAsync();
            _logger.LogInformation("Successfully saved AI roadmap content for track {TrackName}, level {Level}", track.Name, level);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to save AI roadmap content for track {TrackName}, level {Level}", track.Name, level);
            throw;
        }
    }

    private static string GetVideo(Dictionary<string, string>? videos, string key)
    {
        if (videos == null)
            return string.Empty;

        return videos.TryGetValue(key, out var value)
            ? value ?? string.Empty
            : string.Empty;
    }

    private static string NormalizeTrackName(string? trackName)
    {
        return (trackName ?? string.Empty)
            .Trim()
            .ToLower()
            .Replace("-", "_")
            .Replace(" ", "_");
    }

    private static string NormalizeLevel(string? level)
    {
        var value = (level ?? "beginner").Trim().ToLower();

        return value switch
        {
            "junior" => "beginner",
            "mid" => "intermediate",
            "middle" => "intermediate",
            "senior" => "advanced",
            _ => value
        };
    }
}