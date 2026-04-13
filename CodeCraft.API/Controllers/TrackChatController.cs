using CodeCraft.API.Hubs;
using CodeCraft.Application.DTOs.Community;
using CodeCraft.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CodeCraft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TrackChatController : ControllerBase
{
    private readonly ITrackChatService _chatService;
    private readonly IHubContext<TrackChatHub> _hubContext;

    public TrackChatController(
        ITrackChatService chatService,
        IHubContext<TrackChatHub> hubContext)
    {
        _chatService = chatService;
        _hubContext = hubContext;
    }

    [HttpGet("{trackId}")]
    public async Task<IActionResult> GetMessages(int trackId)
    {
        var userId = GetUserId();

        var messages = await _chatService.GetMessagesByTrackAsync(trackId, userId);

        return Ok(messages);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(SendTrackMessageDto dto)
    {
        var userId = GetUserId();

        var message = await _chatService.SendMessageAsync(userId, dto);

        await _hubContext.Clients
            .Group($"track-{dto.TrackId}")
            .SendAsync("ReceiveMessage", message);

        return Ok(message);
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            throw new UnauthorizedAccessException("User not authenticated");

        return int.Parse(userIdClaim.Value);
    }
}