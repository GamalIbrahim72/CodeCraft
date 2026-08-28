using CodeCraft.Application.DTOs;
using CodeCraft.Application.DTOs.Admin;
using CodeCraft.Application.Interfaces.Repositories;
using CodeCraft.Application.Interfaces.Services;
using CodeCraft.Domain.Enums;
using Mapster;

namespace CodeCraft.Application.Services;

public class AdminService : IAdminService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserTrackRepository _userTrackRepository;

    public AdminService(
        IUserRepository userRepository,
        IUserTrackRepository userTrackRepository)
    {
        _userRepository = userRepository;
        _userTrackRepository = userTrackRepository;
    }

    public async Task MakeAdminAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            throw new KeyNotFoundException("User not found");

        user.Role = UserRole.Admin;
        await _userRepository.UpdateAsync(user);
    }

    public async Task<List<UserResponse>> GetUsersAsync(string? search)
    {
        var users = await _userRepository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.Trim();
            users = users.Where(u =>
                (u.FirstName != null && u.FirstName.Contains(searchLower, StringComparison.OrdinalIgnoreCase)) ||
                (u.LastName != null && u.LastName.Contains(searchLower, StringComparison.OrdinalIgnoreCase)) ||
                (u.Email != null && u.Email.Contains(searchLower, StringComparison.OrdinalIgnoreCase)) ||
                u.Id.ToString() == searchLower
            );
        }

        return users.Adapt<List<UserResponse>>();
    }

    public async Task DeleteUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            throw new KeyNotFoundException("User not found");

        await _userRepository.DeleteAsync(user);
    }

    public async Task<List<AdminTrackStatsDto>> GetTrackStatsAsync()
    {
        return await _userTrackRepository.GetTrackStatsAsync();
    }
}
