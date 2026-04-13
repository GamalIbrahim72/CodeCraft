using System.Security.Claims;
using CodeCraft.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CodeCraft.API.Hubs;

[Authorize]
public class TrackChatHub : Hub
{
    private readonly IUserTrackRepository _userTrackRepository;

    public TrackChatHub(IUserTrackRepository userTrackRepository)
    {
        _userTrackRepository = userTrackRepository;
    }

    public async Task JoinTrackGroup(int trackId)
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            throw new HubException("User not authenticated");

        var userId = int.Parse(userIdClaim.Value);

        var isEnrolled = await _userTrackRepository.IsUserEnrolledInTrackAsync(userId, trackId);

        if (!isEnrolled)
            throw new HubException("You are not enrolled in this track");

        await Groups.AddToGroupAsync(Context.ConnectionId, $"track-{trackId}");
    }

    public async Task LeaveTrackGroup(int trackId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"track-{trackId}");
    }
}