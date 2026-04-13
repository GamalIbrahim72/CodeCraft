using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.DTOs.Community;
public class TrackMessageDto
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;
    public DateTime SentAt { get; set; }

    public int SenderId { get; set; }
    public string SenderName { get; set; } = null!;

    public int TrackId { get; set; }
}
