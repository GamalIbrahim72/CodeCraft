using CodeCraft.Application.DTOs;
using CodeCraft.Application.DTOs.Admin;

namespace CodeCraft.Application.Interfaces.Services;

public interface IAdminService
{
    Task MakeAdminAsync(int userId);
    Task<List<UserResponse>> GetUsersAsync(string? search);
    Task DeleteUserAsync(int userId);
    Task<List<AdminTrackStatsDto>> GetTrackStatsAsync();
}
