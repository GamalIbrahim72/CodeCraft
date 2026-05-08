using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.DTOs.AI;
public class AiCourseDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? Level { get; set; }
    public int Order { get; set; }
    public List<AiLessonDto> Lessons { get; set; } = new();
}
