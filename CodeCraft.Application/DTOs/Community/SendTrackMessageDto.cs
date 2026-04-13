using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.DTOs.Community;
public class SendTrackMessageDto
{
    public int TrackId { get; set; }
    public string Content { get; set; } = null!;
}
