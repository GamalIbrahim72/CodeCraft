using CodeCraft.Application.Common.Exceptions;
using CodeCraft.Application.DTOs.AuthDTOs;
using CodeCraft.Application.DTOs.Password;
using CodeCraft.Application.Interfaces.Repositories;
using CodeCraft.Application.Interfaces.Services;
using CodeCraft.Application.Services;
using CodeCraft.Domain.Entities;
using CodeCraft.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace CodeCraft.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _tokenServiceMock = new Mock<ITokenService>();
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _passwordHasherMock = new Mock<IPasswordHasher>();

        _authService = new AuthService(
            _userRepositoryMock.Object,
            _tokenServiceMock.Object,
            _refreshTokenRepositoryMock.Object,
            _emailServiceMock.Object,
            _passwordHasherMock.Object
        );
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ThrowsBadRequestException()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "existing@example.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe",
            Phone = "01000000000"
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync(new User { Email = request.Email });

        // Act
        var act = async () => await _authService.RegisterAsync(request);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Email is already registered");
    }

    [Fact]
    public async Task RegisterAsync_WhenFirstUser_AssignsAdminRoleAndSendsEmail()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "admin@example.com",
            Password = "Password123!",
            FirstName = "Super",
            LastName = "Admin",
            Phone = "01000000000"
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync((User?)null);

        _userRepositoryMock
            .Setup(r => r.CountAsync())
            .ReturnsAsync(0);

        _passwordHasherMock
            .Setup(h => h.Hash(request.Password))
            .Returns("hashed_pwd");

        // Act
        await _authService.RegisterAsync(request);

        // Assert
        _userRepositoryMock.Verify(r => r.AddAsync(It.Is<User>(u =>
            u.Email == request.Email &&
            u.Role == UserRole.Admin &&
            u.PasswordHash == "hashed_pwd" &&
            !u.EmailConfirmed
        )), Times.Once);

        _emailServiceMock.Verify(e => e.SendEmailAsync(
            request.Email,
            It.IsAny<string>(),
            It.IsAny<string>()
        ), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ThrowsUnauthorizedException()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "Password123!"
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _authService.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Invalid email or password");
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsIncorrect_ThrowsUnauthorizedException()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "user@example.com",
            PasswordHash = "correct_hash",
            EmailConfirmed = true
        };

        var request = new LoginRequest
        {
            Email = user.Email,
            Password = "WrongPassword!"
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(h => h.Verify(request.Password, user.PasswordHash))
            .Returns(false);

        // Act
        var act = async () => await _authService.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Invalid email or password");
    }

    [Fact]
    public async Task LoginAsync_WhenEmailNotConfirmed_ThrowsUnauthorizedException()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "unconfirmed@example.com",
            PasswordHash = "correct_hash",
            EmailConfirmed = false
        };

        var request = new LoginRequest
        {
            Email = user.Email,
            Password = "Password123!"
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(h => h.Verify(request.Password, user.PasswordHash))
            .Returns(true);

        // Act
        var act = async () => await _authService.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Please verify your email before login");
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsValid_ReturnsTokenAndRefreshToken()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "valid@example.com",
            PasswordHash = "correct_hash",
            EmailConfirmed = true
        };

        var request = new LoginRequest
        {
            Email = user.Email,
            Password = "Password123!"
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(h => h.Verify(request.Password, user.PasswordHash))
            .Returns(true);

        _tokenServiceMock
            .Setup(t => t.GenerateToken(user))
            .Returns("sample_jwt_token");

        // Act
        var response = await _authService.LoginAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.Token.Should().Be("sample_jwt_token");
        response.RefreshToken.Should().NotBeNullOrWhiteSpace();

        _refreshTokenRepositoryMock.Verify(r => r.AddAsync(It.Is<RefreshToken>(rt =>
            rt.UserId == user.Id &&
            !string.IsNullOrEmpty(rt.Token)
        )), Times.Once);
    }

    [Fact]
    public async Task ResetPassword_WhenPasswordsDoNotMatch_ThrowsBadRequestException()
    {
        // Arrange
        var dto = new ResetPasswordDto
        {
            ResetToken = "token123",
            NewPassword = "Password123!",
            ConfirmPassword = "DifferentPassword123!"
        };

        // Act
        var act = async () => await _authService.ResetPassword(dto);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Passwords do not match");
    }
}
