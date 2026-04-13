using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Domain.Entities;
public class UserLessonProgress
{
    public int Id { get; set; }

    public string UserId { get; set; }

    public int LessonId { get; set; }

    public bool IsCompleted { get; set; }

    public int WatchedSeconds { get; set; }

    public DateTime LastWatchedAt { get; set; }

    public Lesson Lesson { get; set; }
}
