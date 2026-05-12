using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Domain.Entities;
public class Course
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int TrackId { get; set; }
    public int Order { get; set; }
    public Track Track { get; set; } = null!;
    public string Level { get; set; } = null!;
    public string? ExternalTopicId { get; set; }

    public ICollection<Lesson> Lessons { get; set; }= new List<Lesson>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
