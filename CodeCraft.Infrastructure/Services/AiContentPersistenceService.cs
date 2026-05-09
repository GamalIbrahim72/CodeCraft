using CodeCraft.Application.DTOs.AI;
using CodeCraft.Domain.Entities;
using CodeCraft.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CodeCraft.Infrastructure.Services;

public class AiContentPersistenceService : IAiContentPersistenceService
{
    private readonly AppDbContext _context;

    public AiContentPersistenceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveGeneratedContentAsync(AiRoadmapResponseDto data)
    {
        if (data.Data == null)
            throw new Exception("AI roadmap data is null");

        var aiTrackName = NormalizeTrackName(data.Data.Track);

        var track = await _context.Tracks
            .FirstOrDefaultAsync(t =>
                t.Name.ToLower() == aiTrackName ||
                t.Name.ToLower().Replace(" ", "_") == aiTrackName ||
                t.Name.ToLower().Replace(" ", "") == aiTrackName.Replace("_", ""));

        if (track == null)
            throw new Exception($"Track not found: {data.Data.Track}");

        var level = data.Data.Level ?? "Beginner";

        var oldCourses = await _context.Courses
     .Where(c => c.TrackId == track.Id && c.Level.ToLower() == level.ToLower())
     .ToListAsync();

        if (oldCourses.Any())
        {
            var oldCourseIds = oldCourses.Select(c => c.Id).ToList();

            var oldLessons = await _context.Lessons
                .Where(l => oldCourseIds.Contains(l.CourseId))
                .ToListAsync();

            _context.Lessons.RemoveRange(oldLessons);
            _context.Courses.RemoveRange(oldCourses);

            await _context.SaveChangesAsync();
        }

        foreach (var step in data.Data.Roadmap)
        {
            var course = new Course
            {
                Title = step.MainTopic ?? "Untitled Course",
                Description = $"AI Generated Course - Topic Id: {step.TopicId}",
                Order = step.Step,
                TrackId = track.Id,
                Level = level
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var lessonOrder = 1;

            foreach (var lessonDto in step.Lessons)
            {
                var lesson = new Lesson
                {
                    Title = lessonDto.Subtopic ?? lessonDto.Topic ?? "Untitled Lesson",
                    Description = lessonDto.Description ?? string.Empty,
                    VideoUrl = lessonDto.Resources?.Video ?? string.Empty,
                    ArticleUrl = lessonDto.Resources?.Article ?? string.Empty,
                    Order = lessonOrder,
                    CourseId = course.Id
                };
                _context.Lessons.Add(lesson);
                lessonOrder++;
            }

            await _context.SaveChangesAsync();
        }
    }

    private static string NormalizeTrackName(string? trackName)
    {
        return (trackName ?? string.Empty)
            .Trim()
            .ToLower()
            .Replace("-", "_")
            .Replace(" ", "_");
    }
}