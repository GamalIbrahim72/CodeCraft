using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Domain.Entities;
public class Track
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public ICollection<Course> Courses { get; set; }
    public ICollection<TrackChatMessage> ChatMessages { get; set; } = new HashSet<TrackChatMessage>();
}
