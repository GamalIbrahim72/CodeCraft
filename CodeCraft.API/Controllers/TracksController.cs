using CodeCraft.Application.Common;
using CodeCraft.Application.DTOs.TrackDTOs;
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
public class TracksController : BaseController
{
    private readonly ITrackService _trackService;
    private readonly IGenericRepository<Track> _trackRepository;

    public TracksController(ITrackService trackService, IGenericRepository<Track> trackRepository)
    {
        _trackService = trackService;
        _trackRepository = trackRepository;
    }


    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetTracks(string? search)
    {
        IEnumerable<Track> tracks;

        if (!string.IsNullOrEmpty(search))
        {
            tracks = await _trackRepository.FindAsync(t =>
                t.Name.Contains(search)
            );
        }
        else
        {
            tracks = await _trackRepository.GetAllAsync();
        }

        var result = tracks.Adapt<List<TrackResponse>>();

        return Ok(result);
    }



    [AllowAnonymous]

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTrack(int id)
    {
        var track = await _trackService.GetTrack(id);

        if (track == null)
            return NotFound();

        return Ok(track);
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateTrack(TrackRequest request)
    {
        var track = await _trackService.CreateTrack(request);

        return Ok(track);
    }


    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] PaginationParameters parameters)
    {
        var result = await _trackService.GetPagedTracks(parameters);

        return SuccessResponse(result, "Tracks retrieved successfully");
    }


    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateTrackRequest request)
    {
        await _trackService.UpdateTrack(id, request);

        return SuccessResponse<string>(null, "Track updated successfully");
    }


    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _trackService.DeleteTrack(id);

        return SuccessResponse<string>(null, "Track deleted successfully");
    }

    [Authorize]
    [HttpPost("{trackId}/enroll")]
    public async Task<IActionResult> Enroll(int trackId)
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        await _trackService.EnrollAsync(userId, trackId);

        return Ok(new { message = "Enrolled successfully 🔥" });
    }

    [Authorize]
    [HttpGet("my-tracks")]
    public async Task<IActionResult> GetMyTracks()
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        var tracks = await _trackService.GetUserTracksAsync(userId);

        return Ok(tracks);
    }


}
