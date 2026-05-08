using CodeCraft.Application.DTOs.AI;
using CodeCraft.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;

namespace CodeCraft.API.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AiController : BaseController
{
    private readonly IAiRoadmapService _aiRoadmapService;
    private readonly IAiContentPersistenceService _aiContentPersistenceService;
    private readonly IAiService _aiService;
    private readonly IUserRepository _userRepository;
    private readonly ITrackRepository _trackRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IProgressService _progressService;
    private readonly IMemoryCache _cache;
    private readonly IUserTrackRepository _userTrackRepository;
    public AiController(IAiService aiService, IUserRepository userRepository, ITrackRepository trackRepository,IProgressService progressService, IAiRoadmapService aiRoadmapService, IAiContentPersistenceService aiContentPersistenceService , ICourseRepository courseRepository, IMemoryCache cache , IUserTrackRepository userTrackRepository)
    {
        _aiService = aiService;
        _userRepository = userRepository;
        _trackRepository = trackRepository;
        _progressService = progressService;
        _aiRoadmapService = aiRoadmapService;
        _aiContentPersistenceService = aiContentPersistenceService;
        _courseRepository = courseRepository;
        _cache = cache;
        _userTrackRepository = userTrackRepository;

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








    [HttpGet("questions")]
    public async Task<IActionResult> GetQuestions([FromQuery] string track)
    {
        var result = await _aiService.GetQuestionsAsync(track);

        if (result == null)
            return StatusCode(500, "AI questions service failed");

        return Ok(result);
    }

    [HttpPost("evaluate")]
    public async Task<IActionResult> Evaluate([FromBody] EvaluateRequestDto dto)
    {
        var result = await _aiService.EvaluateAsync(dto);

        if (result == null)
            return StatusCode(500, "AI evaluation service failed");

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        var user = await _userRepository.GetByIdAsync(int.Parse(userId));

        if (!string.IsNullOrEmpty(result.Level))
        {
            user.Level = result.Level;
            await _userRepository.UpdateAsync(user);
        }

        return Ok(result);
    }



    [HttpPost("start-roadmap")]
    public async Task<IActionResult> StartRoadmap([FromBody] StartRoadmapDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        var user = await _userRepository.GetByIdAsync(int.Parse(userId));

        user.Level = dto.Level;
        await _userRepository.UpdateAsync(user);

        // لو عندك enroll service/repository استخدمه هنا
        // await _trackRepository.EnrollAsync(user.Id, dto.TrackId);

        var allCourses = await _courseRepository.GetAllAsync();

        var existingCourses = allCourses
            .Where(c => c.TrackId == dto.TrackId && c.Level == dto.Level)
            .OrderBy(c => c.Order)
            .ToList();

        if (!existingCourses.Any())
        {
            var aiResult = await _aiRoadmapService.GenerateRoadmapAsync(dto.TrackId, dto.Level);

            if (aiResult == null)
                return StatusCode(500, "AI failed");

            await _aiContentPersistenceService.SaveGeneratedContentAsync(aiResult);

            allCourses = await _courseRepository.GetAllAsync();

            existingCourses = allCourses
                .Where(c => c.TrackId == dto.TrackId && c.Level == dto.Level)
                .OrderBy(c => c.Order)
                .ToList();
        }

        return Ok(existingCourses);
    }

    [HttpPost("exam/start")]
    public async Task<IActionResult> StartExamStepByStep([FromBody] StartExamRequestDto dto)
    {
        var questions = await _aiService.GetQuestionsAsync(dto.Track);

        if (questions == null || !questions.Any())
            return StatusCode(500, "AI questions service failed");

        var sessionId = Guid.NewGuid().ToString();

        var state = new ExamSessionState
        {
            
            Track = dto.Track,
            Questions = questions,
            CurrentIndex = 0
        };

        _cache.Set(sessionId, state, TimeSpan.FromMinutes(30));

        return Ok(new ExamQuestionResponseDto
        {
            SessionId = sessionId,
            QuestionNumber = 1,
            TotalQuestions = questions.Count,
            Question = questions[0],
            IsFinished = false
        });
    }

    [HttpPost("exam/answer")]
    public async Task<IActionResult> AnswerStepByStep([FromBody] SubmitExamAnswerDto dto)
    {
        if (!_cache.TryGetValue(dto.SessionId, out ExamSessionState? state) || state == null)
            return BadRequest("Invalid or expired exam session");

        var currentQuestion = state.Questions[state.CurrentIndex];

        state.Answers.Add(new AnswerEvaluationDto
        {
            Question_Id = currentQuestion.Question_Id,
            Question_Text = currentQuestion.Question_Text,
            Difficulty_Level = currentQuestion.Difficulty_Level,
            Topic_Area = currentQuestion.Topic_Area,
            Track = currentQuestion.Track,
            Student_Answer = dto.StudentAnswer
        });

        state.CurrentIndex++;

        if (state.CurrentIndex < state.Questions.Count)
        {
            _cache.Set(dto.SessionId, state, TimeSpan.FromMinutes(30));

            return Ok(new ExamQuestionResponseDto
            {
                SessionId = dto.SessionId,
                QuestionNumber = state.CurrentIndex + 1,
                TotalQuestions = state.Questions.Count,
                Question = state.Questions[state.CurrentIndex],
                IsFinished = false
            });
        }

        var evaluateRequest = new EvaluateRequestDto
        {
            Answers = state.Answers
        };

        var result = await _aiService.EvaluateAsync(evaluateRequest);

        _cache.Remove(dto.SessionId);

        if (result == null)
            return StatusCode(500, "AI evaluation service failed");

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        var user = await _userRepository.GetByIdAsync(int.Parse(userId));

        if (!string.IsNullOrEmpty(result.Level))
        {
            user.Level = result.Level;
            await _userRepository.UpdateAsync(user);
        }
       

        return Ok(new
        {
            isFinished = true,
            result
        });
    }
}
