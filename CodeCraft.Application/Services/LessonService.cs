using CodeCraft.Application.Common;
using CodeCraft.Application.DTOs.Lessons;
using CodeCraft.Application.Interfaces.Repositories;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.Services;
public class LessonService: ILessonService
{
    private readonly IGenericRepository<Lesson> _lessonRepository;
    private readonly IMapper _mapper;

    public LessonService(
        IGenericRepository<Lesson> lessonRepository,
        IMapper mapper)
    {
        _lessonRepository = lessonRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LessonResponse>> GetLessonsByCourseId(int courseId)
    {
        var lessons = await _lessonRepository.FindAsync(l => l.CourseId == courseId);

        return _mapper.Map<IEnumerable<LessonResponse>>(lessons);
    }

    public async Task<LessonResponse> CreateLesson(CreateLessonRequest request)
    {
        if (string.IsNullOrEmpty(request.VideoUrl))
            throw new Exception("VideoUrl is required");

        var course = await _lessonRepository.GetByIdAsync(request.CourseId);

        if (course == null)
            throw new Exception("Course not found");

        var lesson = _mapper.Map<Lesson>(request);

        await _lessonRepository.AddAsync(lesson);

        return _mapper.Map<LessonResponse>(lesson);
    }

    public async Task<PaginatedResponse<LessonResponse>> GetPagedLessons(PaginationParameters parameters)
    {
        var (lessons, totalCount) =
            await _lessonRepository.GetPagedAsync(parameters.Page, parameters.PageSize);

        var mapped = _mapper.Map<IEnumerable<LessonResponse>>(lessons);

        return new PaginatedResponse<LessonResponse>
        {
            Data = mapped,
            Page = parameters.Page,
            PageSize = parameters.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<LessonResponse> GetLessonById(int id)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);
        return _mapper.Map<LessonResponse>(lesson);
    }

    public async Task UpdateAsync(UpdateLessonDto dto)
    {
        var lesson = await _lessonRepository.GetByIdAsync(dto.Id);
        if (lesson == null)
            throw new Exception("Lesson not found");
        _mapper.Map(dto, lesson);
        await _lessonRepository.UpdateAsync(lesson);
    }

    public async Task DeleteAsync(int id)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);
        if (lesson == null)
            throw new Exception("Lesson not found");
        await _lessonRepository.DeleteAsync(lesson);
    }

}
