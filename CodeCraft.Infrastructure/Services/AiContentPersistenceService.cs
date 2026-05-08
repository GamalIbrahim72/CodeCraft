using CodeCraft.Application.DTOs.AI;
using CodeCraft.Infrastructure.Persistence;
using Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Infrastructure.Services;
public class AiContentPersistenceService: IAiContentPersistenceService
{
    private readonly AppDbContext _context;

    public AiContentPersistenceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveGeneratedContentAsync(AiRoadmapResponseDto data)
    {
        var track = await _context.Tracks.FindAsync(data.TrackId);

        if (track == null)
            throw new Exception("Track not found");

        foreach (var courseDto in data.Courses)
        {
            var course = new Course
            {
                Title = courseDto.Title,
                Description = courseDto.Description,
                Order = courseDto.Order,
                TrackId = data.TrackId,
                Level = courseDto.Level ?? "Beginner"
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync(); // عشان نجيب Id

            foreach (var lessonDto in courseDto.Lessons)
            {
                var lesson = new Lesson
                {
                    Title = lessonDto.Title,
                    Description = lessonDto.Description,
                    VideoUrl = lessonDto.VideoUrl,
                    Duration = lessonDto.Duration,
                    Order = lessonDto.Order,
                    CourseId = course.Id
                };

                _context.Lessons.Add(lesson);
                await _context.SaveChangesAsync();

                foreach (var attDto in lessonDto.Attachments)
                {
                    var attachment = new LessonAttachment
                    {
                        FileName = attDto.FileName,
                        FileUrl = attDto.FileUrl,
                        LessonId = lesson.Id
                    };

                    _context.LessonAttachments.Add(attachment);
                }
            }
        }

        await _context.SaveChangesAsync();
    }
}
