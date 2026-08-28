using CodeCraft.Application.Common;
using CodeCraft.Application.DTOs.CoursesDTOs;
using CodeCraft.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeCraft.API.Controllers;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CoursesController : BaseController
{
    private readonly ICourseService _courseService;
    private readonly IGenericRepository<Course> _courseRepository;
    private readonly IUserRepository _userRepository;
    public CoursesController(ICourseService courseService, IGenericRepository<Course> courseRepository, IUserRepository userRepository)
    {
        _courseService = courseService;
        _courseRepository = courseRepository;
        _userRepository = userRepository;
    }


    [HttpGet]
    public async Task<IActionResult> GetCourses(string? search)
    {
        IEnumerable<Course> courses;

        if (!string.IsNullOrEmpty(search))
        {
            courses = await _courseRepository.FindAsync(c =>
                c.Title.Contains(search) ||
                c.Description.Contains(search)
            );
        }
        else
        {
            courses = await _courseRepository.GetAllAsync();
        }

        var result = courses.Adapt<List<CourseResponse>>();

        return Ok(result);
    }

  
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _courseService.GetCourseById(id);
        return SuccessResponse(result);
    }



    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateCourseRequest request)
    {
        var course = await _courseService.CreateCourse(request);

        return Ok(course);
    }



  
    [HttpGet("track/{trackId}")]
    public async Task<IActionResult> GetByTrack(int trackId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        var user = await _userRepository.GetByIdAsync(int.Parse(userId));

        if (user == null)
            return NotFound("User not found");

        var courses = await _courseRepository.GetAllAsync();

        var result = courses
            .Where(c => c.TrackId == trackId && c.Level == user.Level)
            .OrderBy(c => c.Order);
        return Ok(result);
    }


    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] PaginationParameters parameters)
    {
        var result = await _courseService.GetPagedCourses(parameters);

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateCourseRequest request)
    {
        await _courseService.UpdateCourse(id, request);
        return SuccessResponse<string>(null, "Course updated successfully");
    }

   
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _courseService.DeleteCourse(id);
        return SuccessResponse<string>(null, "Course deleted successfully");
    }

}
