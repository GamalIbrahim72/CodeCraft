using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.DTOs.Progress;
public class UpdateWatchTimeRequest
{
    public int LessonId { get; set; }

    public int WatchedSeconds { get; set; }
}
