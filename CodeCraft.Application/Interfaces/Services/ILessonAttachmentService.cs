using CodeCraft.Application.DTOs.Attachment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.Interfaces.Services;
public interface ILessonAttachmentService
{
    Task<IEnumerable<AttachmentDto>> GetByLessonIdAsync(int lessonId);

    Task<AttachmentDto> UploadAsync(CreateAttachmentDto dto);

    Task DeleteAsync(int id);
}
