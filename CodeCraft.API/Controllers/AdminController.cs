using CodeCraft.Application.DTOs;
using CodeCraft.Application.DTOs.Admin;
using CodeCraft.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeCraft.API.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class AdminController : BaseController
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpPost("make-admin/{userId}")]
    public async Task<IActionResult> MakeAdmin(int userId)
    {
        await _adminService.MakeAdminAsync(userId);
        return SuccessResponse<string>(null, "User promoted to Admin");
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] string? search)
    {
        var users = await _adminService.GetUsersAsync(search);
        return Ok(users);
    }

    [HttpDelete("user/{userId}")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        await _adminService.DeleteUserAsync(userId);
        return SuccessResponse<string>(null, "User deleted successfully");
    }

    [HttpGet("track-stats")]
    public async Task<IActionResult> GetTrackStats()
    {
        var stats = await _adminService.GetTrackStatsAsync();
        return Ok(stats);
    }
}