using CodeCraft.Application.Common;
using CodeCraft.Application.DTOs.CoursesDTOs;
using CodeCraft.Application.Interfaces.Repositories;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.Services;
public class CourseService: ICourseService
{
    //private readonly IGenericRepository<Course> _courseRepository;
    private readonly IMapper _mapper;
    private readonly ICourseRepository _courseRepository;

    public CourseService(ICourseRepository courseRepository,IMapper mapper)
       
    {
        _courseRepository = courseRepository;
        _mapper = mapper;

    }

    public async Task<IEnumerable<CourseResponse>> GetAllCourses()
    {
        var courses = await _courseRepository.GetAllAsync();

        return _mapper.Map<IEnumerable<CourseResponse>>(courses);
    }

    public async Task<CourseResponse> CreateCourse(CreateCourseRequest request)
    {
        var order = await _courseRepository.GetNextOrder(request.TrackId);

        var course = _mapper.Map<Course>(request);

        course.Order = order;

        await _courseRepository.AddAsync(course);

        return _mapper.Map<CourseResponse>(course);
    }

    public async Task<IEnumerable<CourseResponse>> GetCoursesByTrackId(int trackId)
    {
        var courses = await _courseRepository.FindAsync(c => c.TrackId == trackId);

        return _mapper.Map<IEnumerable<CourseResponse>>(courses);
    }

    public async Task<PaginatedResponse<CourseResponse>> GetPagedCourses(PaginationParameters parameters)
    {
        var (courses, totalCount) =
            await _courseRepository.GetPagedAsync(parameters.Page, parameters.PageSize);

        var mapped = _mapper.Map<IEnumerable<CourseResponse>>(courses);

        return new PaginatedResponse<CourseResponse>
        {
            Data = mapped,
            Page = parameters.Page,
            PageSize = parameters.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task UpdateCourse(int id, UpdateCourseRequest request)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if (course == null)
            throw new Exception("Course not found");

        _mapper.Map(request, course);

        await _courseRepository.UpdateAsync(course);
    }

    public async Task DeleteCourse(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if (course == null)
            throw new Exception("Course not found");

        await _courseRepository.DeleteAsync(course);
    }

    public async Task<CourseResponse> GetCourseById(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if (course == null)
            throw new Exception("Course not found");

        return _mapper.Map<CourseResponse>(course);
    }




}
