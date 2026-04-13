using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Domain.Entities;
public class Course
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public int TrackId { get; set; }
    public int Order { get; set; }
    public Track Track { get; set; }
    public string Level { get; set; }

    public ICollection<Lesson> Lessons { get; set; }= new List<Lesson>();
    public ICollection<Enrollment> Enrollments { get; set; }
}
