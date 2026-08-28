using CodeCraft.Application.DTOs.Admin;
using CodeCraft.Application.Interfaces.Repositories;
using CodeCraft.Application.Services;
using CodeCraft.Domain.Entities;
using CodeCraft.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace CodeCraft.Tests.Services;

public class AdminServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserTrackRepository> _userTrackRepositoryMock;
    private readonly AdminService _adminService;

    public AdminServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userTrackRepositoryMock = new Mock<IUserTrackRepository>();

        _adminService = new AdminService(
            _userRepositoryMock.Object,
            _userTrackRepositoryMock.Object
        );
    }

    [Fact]
    public async Task MakeAdminAsync_WhenUserNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _adminService.MakeAdminAsync(999);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("User not found");
    }

    [Fact]
    public async Task MakeAdminAsync_WhenUserExists_UpdatesRoleToAdmin()
    {
        // Arrange
        var user = new User
        {
            Id = 10,
            Email = "user@test.com",
            Role = UserRole.User
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        // Act
        await _adminService.MakeAdminAsync(user.Id);

        // Assert
        user.Role.Should().Be(UserRole.Admin);
        _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task GetUsersAsync_WhenFilteredBySearch_ReturnsMatchingUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, FirstName = "Ahmed", LastName = "Ali", Email = "ahmed@example.com" },
            new User { Id = 2, FirstName = "Mohamed", LastName = "Hassan", Email = "mohamed@example.com" },
            new User { Id = 3, FirstName = "Sara", LastName = "Ibrahim", Email = "sara@example.com" }
        };

        _userRepositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(users);

        // Act
        var result = await _adminService.GetUsersAsync("ahmed");

        // Assert
        result.Should().HaveCount(1);
        result[0].Email.Should().Be("ahmed@example.com");
    }

    [Fact]
    public async Task DeleteUserAsync_WhenUserNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _adminService.DeleteUserAsync(999);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("User not found");
    }

    [Fact]
    public async Task DeleteUserAsync_WhenUserExists_DeletesUser()
    {
        // Arrange
        var user = new User { Id = 5, Email = "delete@example.com" };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        // Act
        await _adminService.DeleteUserAsync(user.Id);

        // Assert
        _userRepositoryMock.Verify(r => r.DeleteAsync(user), Times.Once);
    }

    [Fact]
    public async Task GetTrackStatsAsync_ReturnsRepositoryData()
    {
        // Arrange
        var stats = new List<AdminTrackStatsDto>
        {
            new AdminTrackStatsDto
            {
                TrackId = 1,
                TrackName = "Backend .NET",
                TotalUsers = 25,
                Levels = new List<AdminLevelStatsDto>
                {
                    new AdminLevelStatsDto { Level = "beginner", UsersCount = 15 },
                    new AdminLevelStatsDto { Level = "intermediate", UsersCount = 10 }
                }
            }
        };

        _userTrackRepositoryMock
            .Setup(r => r.GetTrackStatsAsync())
            .ReturnsAsync(stats);

        // Act
        var result = await _adminService.GetTrackStatsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].TrackName.Should().Be("Backend .NET");
        result[0].TotalUsers.Should().Be(25);
    }
}
