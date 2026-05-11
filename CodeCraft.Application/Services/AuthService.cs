using CodeCraft.Application.Common;
using CodeCraft.Application.Common.Exceptions;
using CodeCraft.Application.DTOs.AuthDTOs;
using CodeCraft.Application.DTOs.Password;
using CodeCraft.Application.Interfaces.Repositories;
using CodeCraft.Application.Interfaces.Services;
using CodeCraft.Domain.Enums;
using Google.Apis.Auth;
using Newtonsoft.Json;

namespace CodeCraft.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IEmailService _emailService;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(
        IUserRepository userRepository,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IEmailService emailService,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _emailService = emailService;
        _passwordHasher = passwordHasher;
    }

    private string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);

        if (existingUser != null)
            throw new BadRequestException("Email is already registered");

        var verificationCode = new Random()
            .Next(100000, 999999)
            .ToString();

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            DateOfBirth = request.DateOfBirth,
            PasswordHash = Hash(request.Password),

            EmailConfirmed = false,
            EmailVerificationCode = verificationCode,
            EmailVerificationCodeExpiry = DateTime.UtcNow.AddMinutes(10)
        };

        var usersCount = await _userRepository.CountAsync();

        user.Role = usersCount == 0 ? UserRole.Admin : UserRole.User;

        await _userRepository.AddAsync(user);

        await _emailService.SendEmailAsync(
            user.Email,
            "CodeCraft Email Verification",
            $@"
            <h2>Verify Your Email</h2>
            <p>Your verification code is:</p>
            <h1>{verificationCode}</h1>
            <p>This code will expire in 10 minutes.</p>
            "
        );
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
            throw new UnauthorizedException("Invalid email or password");

        var passwordValid = Verify(request.Password, user.PasswordHash);

        if (!passwordValid)
            throw new UnauthorizedException("Invalid email or password");

        if (!user.EmailConfirmed)
            throw new UnauthorizedException("Please verify your email before login");

        var token = _tokenService.GenerateToken(user);
        var refreshToken = GenerateRefreshToken();

        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        return new AuthResponse
        {
            Token = token,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResponse> GoogleLoginAsync(string idToken)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);

        var user = await _userRepository.GetByEmailAsync(payload.Email);

        if (user == null)
        {
            user = new User
            {
                Email = payload.Email,
                FirstName = payload.GivenName,
                LastName = payload.FamilyName,
                Role = UserRole.User,
                Provider = "Google",
                ProviderId = payload.Subject,
                EmailConfirmed = true
            };

            await _userRepository.AddAsync(user);
        }
        else
        {
            if (user.Provider == null)
            {
                user.Provider = "Google";
                user.ProviderId = payload.Subject;
                user.EmailConfirmed = true;

                await _userRepository.UpdateAsync(user);
            }
        }

        var token = _tokenService.GenerateToken(user);

        return new AuthResponse
        {
            Token = token
        };
    }

    public async Task<AuthResponse> FacebookLoginAsync(string accessToken)
    {
        using var http = new HttpClient();

        var response = await http.GetAsync(
            $"https://graph.facebook.com/me?fields=id,name,email&access_token={accessToken}"
        );

        if (!response.IsSuccessStatusCode)
            throw new Exception("Invalid Facebook token");

        var content = await response.Content.ReadAsStringAsync();

        dynamic data = JsonConvert.DeserializeObject(content)!;

        string email = data.email;
        string name = data.name;

        if (email == null)
            throw new Exception("Facebook account has no email");

        var nameParts = name.Split(' ');

        var firstName = nameParts[0];
        var lastName = nameParts.Length > 1 ? nameParts[1] : "";

        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            user = new User
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Role = UserRole.User,
                Provider = "Facebook",
                ProviderId = data.id,
                EmailConfirmed = true
            };

            await _userRepository.AddAsync(user);
        }
        else
        {
            if (user.Provider == null)
            {
                user.Provider = "Facebook";
                user.ProviderId = data.id;
                user.EmailConfirmed = true;

                await _userRepository.UpdateAsync(user);
            }
        }

        var token = _tokenService.GenerateToken(user);

        return new AuthResponse
        {
            Token = token
        };
    }

    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }

    public async Task ForgotPassword(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
            return;

        var token = new Random().Next(100000, 999999).ToString();

        user.ResetToken = token;
        user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(10);

        await _userRepository.UpdateAsync(user);

        await _emailService.SendEmailAsync(
            email,
            "Reset Password",
            $"Your code: {token}");
    }

    public async Task ResetPassword(ResetPasswordDto dto)
    {
        if (dto.NewPassword != dto.ConfirmPassword)
            throw new BadRequestException("Passwords do not match");

        var user = await _userRepository.GetByResetTokenAsync(dto.ResetToken);

        if (user == null || user.ResetTokenExpiry < DateTime.UtcNow)
            throw new BadRequestException("Invalid or expired token");

        user.PasswordHash = _passwordHasher.Hash(dto.NewPassword);

        user.ResetToken = null;
        user.ResetTokenExpiry = null;

        await _userRepository.UpdateAsync(user);
    }
}