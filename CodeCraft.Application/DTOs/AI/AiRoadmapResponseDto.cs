using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.DTOs.AI;
public class AiRoadmapResponseDto
{
    public int TrackId { get; set; }
    public List<AiCourseDto> Courses { get; set; } = new();
}
