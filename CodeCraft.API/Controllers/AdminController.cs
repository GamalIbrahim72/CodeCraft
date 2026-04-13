using CodeCraft.Application.DTOs;
using CodeCraft.Domain.Enums;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CodeCraft.API.Controllers;
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class AdminController : BaseController
{
    private readonly IUserRepository _userRepository;
    public IEmailService _emailService;

    public AdminController(IUserRepository userRepository, IEmailService emailService)
    {
        _userRepository = userRepository;
        _emailService = emailService;
    }

    [HttpPost("make-admin/{userId}")]
    public async Task<IActionResult> MakeAdmin(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            return NotFound();

        user.Role = UserRole.Admin;

        await _userRepository.UpdateAsync(user);

        return SuccessResponse<string>(null, "User promoted to Admin ");
    }


    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(string? search)
    {
        var users = await _userRepository.GetAllAsync();
        
        if (!string.IsNullOrEmpty(search))
        {
            users = users.Where(u =>
                u.FirstName.Contains(search) ||
                u.Email.Contains(search) ||
                u.Id.ToString() == search
            );
        }

        var result = users.Adapt<List<UserResponse>>();

        return Ok(result);
    }


    [HttpDelete("user/{userId}")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            return NotFound("User not found");

        await _userRepository.DeleteAsync(user);

        return SuccessResponse<string>(null, "User deleted successfully ");
    }


    
}
