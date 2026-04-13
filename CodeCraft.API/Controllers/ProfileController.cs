using CodeCraft.Application.DTOs;
using CodeCraft.Application.DTOs.Profile;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeCraft.API.Controllers;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProfileController : BaseController
{
    private readonly IUserRepository _userRepository;

    public ProfileController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = int.Parse(
         User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            return NotFound();

        var result = user.Adapt<UserResponse>();

        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
    {
        var userId = int.Parse(
         User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

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
        var userId = int.Parse(
         User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            return NotFound();

        if (file == null || file.Length == 0)
            return BadRequest("Invalid file");

        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

        var path = Path.Combine("wwwroot/profile-images", fileName);

        Directory.CreateDirectory("wwwroot/profile-images");

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        user.ProfileImageUrl = $"/profile-images/{fileName}";

        await _userRepository.UpdateAsync(user);

        return Ok(new { user.ProfileImageUrl });
    }




}
