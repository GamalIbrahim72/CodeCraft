using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.Interfaces.Repositories;
public interface ILessonAttachmentRepository
{
    Task AddAsync(LessonAttachment attachment);

    Task<IEnumerable<LessonAttachment>> GetByLessonIdAsync(int lessonId);

    Task<LessonAttachment> GetByIdAsync(int id);

    void Delete(LessonAttachment attachment);
}
