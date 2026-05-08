using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.DTOs.AI;
public class AiLessonDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public int Duration { get; set; }
    public int Order { get; set; }
    public List<AiLessonAttachmentDto> Attachments { get; set; } = new();
}
