using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Domain.Entities;
public class Lesson
{
    public int Id { get; set; }

    public string Title { get; set; } 

    public string Description { get; set; } = string.Empty;

    public string VideoUrl { get; set; }= string.Empty;
    public string VideoUrl2 { get; set; } = string.Empty;
    public string VideoUrl3 { get; set; } = string.Empty;

    public int Duration { get; set; }

    public int Order { get; set; }

    public int CourseId { get; set; }
    public string ArticleUrl { get; set; } = string.Empty;

    public Course Course { get; set; }

    public ICollection<LessonAttachment> Attachments { get; set; }
}
