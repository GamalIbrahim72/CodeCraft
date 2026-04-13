using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Domain.Entities;
public class TrackChatMessage
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public int SenderId { get; set; }
    public User Sender { get; set; } = null!;

    public int TrackId { get; set; }
    public Track Track { get; set; } = null!;
}
