using CodeCraft.Application.DTOs;
using CodeCraft.Application.DTOs.Profile;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeCraft.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProfileController : BaseController
{
    private readonly IUserRepository _userRepository;
    private readonly IUserTrackRepository _userTrackRepository;

    public ProfileController(
        IUserRepository userRepository,
        IUserTrackRepository userTrackRepository)
    {
        _userRepository = userRepository;
        _userTrackRepository = userTrackRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim))
            return Unauthorized();

        if (!int.TryParse(userIdClaim, out var userId))
            return BadRequest("Invalid user id");

        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            return NotFound();

        var result = user.Adapt<UserResponse>();

        var userTracks = await _userTrackRepository.GetByUserIdAsync(userId);

        var tracks = userTracks.Select(ut => new
        {
            trackId = ut.TrackId,
            trackName = ut.Track?.Name
        }).ToList();

        return Ok(new
        {
            profile = result,
            level = user.Level,
            tracks
        });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim))
            return Unauthorized();

        if (!int.TryParse(userIdClaim, out var userId))
            return BadRequest("Invalid user id");

        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            return NotFound();

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;

        await _userRepository.UpdateAsync(user);

        return SuccessResponse<string>(null, "Profile updated successfully 🔥");
    }

    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim))
            return Unauthorized();

        if (!int.TryParse(userIdClaim, out var userId))
            return BadRequest("Invalid user id");

        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            return NotFound();

        if (file == null || file.Length == 0)
            return BadRequest("Invalid file");

        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

        Directory.CreateDirectory("wwwroot/profile-images");

        var path = Path.Combine("wwwroot/profile-images", fileName);

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        user.ProfileImageUrl = $"/profile-images/{fileName}";

        await _userRepository.UpdateAsync(user);

        return Ok(new { user.ProfileImageUrl });
    }
}