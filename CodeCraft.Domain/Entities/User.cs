using CodeCraft.Domain.Common;
using CodeCraft.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Domain.Entities;
public class User:BaseEntity
{

    public  string ?FirstName { get; set; }

    public  string? LastName { get; set; }
            
    public required string Email { get; set; }

    public  string? PasswordHash { get; set; }

    public  string ?Phone { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string? Level { get; set; }

    public string? DailyGoal { get; set; }
    public string? ProfileImageUrl { get; set; }

    public string? Provider { get; set; }
    public string? ProviderId { get; set; }
    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpiry { get; set; }

    public UserRole Role { get; set; } = UserRole.User;
    public ICollection<UserLessonProgress> Progress { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; }

    public ICollection<TrackChatMessage> TrackChatMessages { get; set; } = new HashSet<TrackChatMessage>();

}
