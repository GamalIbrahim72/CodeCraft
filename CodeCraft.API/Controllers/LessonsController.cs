using CodeCraft.Application.Common;
using CodeCraft.Application.DTOs.Lessons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeCraft.API.Controllers;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class LessonsController : BaseController
{
    private readonly ILessonService _lessonService;

    public LessonsController(ILessonService lessonService)
    {
        _lessonService = lessonService;
    }
    [AllowAnonymous]
    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetByCourse(int courseId)
    {
        var lessons = await _lessonService.GetLessonsByCourseId(courseId);

        return SuccessResponse(lessons, "Lessons retrieved successfully");
    }
    [Authorize (Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateLessonRequest request)
    {
        var lesson = await _lessonService.CreateLesson(request);

        return SuccessResponse(lesson, "Lesson created successfully");
    }

   
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] PaginationParameters parameters)
    {
        var result = await _lessonService.GetPagedLessons(parameters);

        return SuccessResponse(result, "Lessons retrieved successfully");
    }
    [Authorize (Roles ="Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(UpdateLessonDto dto)
    {
        await _lessonService.UpdateAsync(dto);
        return SuccessResponse<string>(null, "Lesson updated successfully");
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _lessonService.DeleteAsync(id);
        return Ok("Deleted");
    }

}
