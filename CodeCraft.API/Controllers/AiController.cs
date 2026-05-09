using CodeCraft.Application.DTOs.AI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace CodeCraft.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AiController : BaseController
{
    private readonly IAiService _aiService;
    private readonly IAiRoadmapService _aiRoadmapService;
    private readonly IAiContentPersistenceService _aiContentPersistenceService;
    private readonly IUserRepository _userRepository;
    private readonly IMemoryCache _cache;

    public AiController(
        IAiService aiService,
        IAiRoadmapService aiRoadmapService,
        IAiContentPersistenceService aiContentPersistenceService,
        IUserRepository userRepository,
        IMemoryCache cache)
    {
        _aiService = aiService;
        _aiRoadmapService = aiRoadmapService;
        _aiContentPersistenceService = aiContentPersistenceService;
        _userRepository = userRepository;
        _cache = cache;
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

        return Ok(new
        {
            message = "Level updated successfully",
            level = user.Level
        });
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

    [HttpGet("roadmap")]
    public async Task<IActionResult> GenerateRoadmap(
        [FromQuery] string track,
        [FromQuery] string level)
    {
        if (string.IsNullOrWhiteSpace(track))
            return BadRequest("Track is required");

        if (string.IsNullOrWhiteSpace(level))
            return BadRequest("Level is required");

        var result = await _aiRoadmapService.GenerateRoadmapAsync(track, level);

        if (result == null)
            return StatusCode(500, "AI roadmap service failed");

        await _aiContentPersistenceService.SaveGeneratedContentAsync(result);

        return Ok(new
        {
            message = "Roadmap generated and saved successfully",
            result
        });
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

        state.Answers.Add(new EvaluateAnswerDto
        {
            QuestionId = currentQuestion.Question_Id,
            StudentAnswer = dto.StudentAnswer
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