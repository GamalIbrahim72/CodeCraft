using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.DTOs.Lessons;
public class LessonResponse
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string VideoUrl { get; set; } = string.Empty;

    public string ArticleUrl { get; set; } = string.Empty;
    public int Duration { get; set; }

    public int Order { get; set; }

    public int CourseId { get; set; }
}
