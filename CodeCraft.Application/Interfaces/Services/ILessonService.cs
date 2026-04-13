using CodeCraft.Application.Common;
using CodeCraft.Application.DTOs.Lessons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.Interfaces.Services;
public interface ILessonService
{

    Task<IEnumerable<LessonResponse>> GetLessonsByCourseId(int courseId);
    Task<LessonResponse> GetLessonById(int id);
    Task UpdateAsync(UpdateLessonDto dto);
    Task DeleteAsync(int id);

    Task<LessonResponse> CreateLesson(CreateLessonRequest request);
    Task<PaginatedResponse<LessonResponse>> GetPagedLessons(PaginationParameters parameters);
}
