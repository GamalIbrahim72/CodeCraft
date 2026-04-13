using CodeCraft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Infrastructure.Persistence;
public class AppDbContext:DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
       : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly
        );
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Track> Tracks { get; set; }

    public DbSet<Course> Courses { get; set; }

    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<UserLessonProgress> UserLessonProgresses { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }

    public DbSet<Quiz> Quizzes { get; set; }

    public DbSet<LessonAttachment> LessonAttachments { get; set; }
    public DbSet<UserTrack> UserTracks { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<TrackChatMessage> TrackChatMessages { get; set; }
}
