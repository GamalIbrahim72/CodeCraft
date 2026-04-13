using CodeCraft.Application.DTOs.AI;
using CodeCraft.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeCraft.API.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AiController : BaseController
{

    private readonly IAiService _aiService;
    private readonly IUserRepository _userRepository;
    private readonly ITrackRepository _trackRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IProgressService _progressService;
    public AiController(IAiService aiService, IUserRepository userRepository, ITrackRepository trackRepository,IProgressService progressService)
    {
        _aiService = aiService;
        _userRepository = userRepository;
        _trackRepository = trackRepository;
        _progressService = progressService;
    }


    [HttpPost("set-level")]
    public async Task<IActionResult> SetLevel([FromBody] SetLevelDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        if (!int.TryParse(userId, out int parsedId))
            return BadRequest("Invalid user id");

        var user = await _userRepository.GetByIdAsync(parsedId);

        user.Level = dto.Level;

        await _userRepository.UpdateAsync(user);

        return Ok();
    }




    [HttpGet("recommended-courses/{trackId}")]
    public async Task<IActionResult> GetRecommendedCourses(int trackId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        var user = await _userRepository.GetByIdAsync(int.Parse(userId));

        var courses = await _courseRepository.GetCoursesByTrackId(trackId);

        var result = courses.Where(c => c.Level == user.Level);

        return Ok(result);
    }

    [HttpGet("start-course/{trackId}")]
    public async Task<IActionResult> GetStartCourse(int trackId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        var user = await _userRepository.GetByIdAsync(int.Parse(userId));

        var courses = await _courseRepository.GetCoursesByTrackId(trackId);


        var startCourse = courses
     .Where(c => c.Level == user.Level)
     .OrderBy(c => c.Order)
     .FirstOrDefault();

        if (startCourse == null)
            return NotFound("No course found for your level");

        return Ok(startCourse);
    }

    [HttpPost("start-exam")]
    public async Task<IActionResult> StartExam()
    {
        var result = await _aiService.StartExam();
        return Ok(result);
    }

    [HttpPost("answer")]
    public async Task<IActionResult> Answer([FromBody] AnswerDto dto)
    {
        var result = await _aiService.SendAnswer(dto);

        // لو AI رجع Level
        if (!string.IsNullOrEmpty(result.Level))
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var user = await _userRepository.GetByIdAsync(int.Parse(userId));

            // ✅ حفظ المستوى
            user.Level = result.Level;
            await _userRepository.UpdateAsync(user);

            // ✅ هات الكورسات
            var courses = await _courseRepository.GetCoursesByTrackId(dto.TrackId);

            var startCourse = courses
                .Where(c => c.Level == user.Level)
                .OrderBy(c => c.Order)
                .FirstOrDefault();

            if (startCourse == null)
                return NotFound("No course found for your level");

            // 🔥🔥🔥 أهم خطوة
            // سجل بداية الكورس في Progress
            await _progressService.StartCourse(user.Id, startCourse.Id);

            return Ok(new
            {
                level = result.Level,
                startCourse = startCourse
            });
        }

        // لسه فيه سؤال
        return Ok(result);
    }


}
