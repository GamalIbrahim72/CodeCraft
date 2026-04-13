using CodeCraft.Application.Common;
using CodeCraft.Application.DTOs.CoursesDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.Interfaces.Services;
public interface ICourseService
{
    Task<IEnumerable<CourseResponse>> GetAllCourses();

    Task<CourseResponse> CreateCourse(CreateCourseRequest request);
    Task<IEnumerable<CourseResponse>> GetCoursesByTrackId(int trackId);

    Task UpdateCourse(int id, UpdateCourseRequest request);

    Task DeleteCourse(int id);
    Task<CourseResponse> GetCourseById(int id);

    Task<PaginatedResponse<CourseResponse>> GetPagedCourses(PaginationParameters parameters);
}
