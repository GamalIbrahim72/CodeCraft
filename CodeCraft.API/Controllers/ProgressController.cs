using CodeCraft.Application.DTOs.Progress;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeCraft.API.Controllers;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProgressController : BaseController
{
    private readonly IProgressService _progressService;

    public ProgressController(IProgressService progressService)
    {
        _progressService = progressService;
    }

    [HttpPost("complete")]
    public async Task<IActionResult> CompleteLesson(int lessonId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        await _progressService.MarkLessonComplete(lessonId, userId);

        return SuccessResponse<string>(null, "Lesson completed");
    }


    [HttpPost("watch-time")]
    public async Task<IActionResult> UpdateWatchTime(UpdateWatchTimeRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        await _progressService.UpdateWatchTime(request, userId);

        return SuccessResponse<string>(null, "Watch time updated");
    }

    [HttpGet("watch-time/{lessonId}")]
    public async Task<IActionResult> GetWatchTime(int lessonId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        var seconds = await _progressService.GetWatchTime(lessonId, userId);

        return SuccessResponse(seconds, "Watch time retrieved");
    }

    [HttpGet("track/{trackId}")]
    public async Task<IActionResult> GetTrackProgress(int trackId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        var progress = await _progressService.GetTrackProgress(trackId, userId);

        return SuccessResponse(progress, "Track progress retrieved");
    }


    [HttpGet("test-auth")]
    [Authorize]
    public IActionResult TestAuth()
    {
        return Ok("Authorized ✔");
    }


    [Authorize]
    [HttpGet("continue")]
    public async Task<IActionResult> ContinueLearning()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var lesson = await _progressService.GetContinueLearningAsync(userId);

        if (lesson == null)
            return NotFound("No progress yet");

        return Ok(lesson);
    }

    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetCourseProgress(int courseId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        var progress = await _progressService
            .GetCourseProgress(courseId, userId);

        return Ok(new { progress });
    }

}
