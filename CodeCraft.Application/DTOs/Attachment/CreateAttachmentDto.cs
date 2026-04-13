using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.DTOs.Attachment;
public class CreateAttachmentDto
{
    public string FileName { get; set; } = string.Empty;

    public Stream? FileStream { get; set; }

    public int LessonId { get; set; }
}
