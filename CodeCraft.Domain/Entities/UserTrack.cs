using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Domain.Entities;
public class UserTrack
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public int TrackId { get; set; }
    public Track Track { get; set; }

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
}