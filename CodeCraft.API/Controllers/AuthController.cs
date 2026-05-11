using CodeCraft.Application.Common;
using CodeCraft.Application.DTOs.AuthDTOs;
using CodeCraft.Application.DTOs.Password;
using CodeCraft.Application.Interfaces.Services;
using CodeCraft.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeCraft.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : BaseController
{
        private readonly IAuthService _authService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;

    public AuthController(IAuthService authService, IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository, ITokenService tokenService, IPasswordHasher passwordHasher, IEmailService emailService)
    {
        _authService = authService;
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _tokenService = tokenService;
        _emailService = emailService;
        _passwordHasher = passwordHasher;
    }

    //register endpoint

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingUser = await _userRepository.GetByEmailAsync(request.Email);

        if (existingUser != null)
            return BadRequest(new
            {
                message = "Email is already registered"
            });

        if (!string.IsNullOrWhiteSpace(request.Phone))
        {
            var existingPhone = await _userRepository.GetByPhoneAsync(request.Phone);

            if (existingPhone != null)
                return BadRequest(new
                {
                    message = "Phone number is already registered"
                });
        }

        await _authService.RegisterAsync(request);

        return SuccessResponse<string>(
            null,
            "User registered successfully. Please check your email to verify your account.");
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);

        if (user == null)
            return BadRequest(new { message = "Invalid email" });

        if (user.EmailConfirmed)
            return BadRequest(new { message = "Email is already verified" });

        if (user.EmailVerificationCode != dto.Code ||
            user.EmailVerificationCodeExpiry < DateTime.UtcNow)
        {
            return BadRequest(new { message = "Invalid or expired verification code" });
        }

        user.EmailConfirmed = true;
        user.EmailVerificationCode = null;
        user.EmailVerificationCodeExpiry = null;

        await _userRepository.UpdateAsync(user);

        return Ok(new
        {
            message = "Email verified successfully"
        });
    }

    [HttpPost("resend-email-code")]
    public async Task<IActionResult> ResendEmailVerificationCode([FromBody] string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
            return BadRequest(new { message = "Invalid email" });

        if (user.EmailConfirmed)
            return BadRequest(new { message = "Email is already verified" });

        var code = new Random().Next(100000, 999999).ToString();

        user.EmailVerificationCode = code;
        user.EmailVerificationCodeExpiry = DateTime.UtcNow.AddMinutes(10);

        await _userRepository.UpdateAsync(user);

        await _emailService.SendEmailAsync(
    user.Email,
    "CodeCraft Verification Code",
    $"Your verification code is: {code}"
);

        return Ok(new
        {
            message = "Verification code sent successfully"
        });
    }

    //login endpoint
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        return SuccessResponse(result, "User login successfully");
    }

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        var result = await _authService.GoogleLoginAsync(request.IdToken);

        return Ok(result);
    }

    [HttpPost("facebook-login")]
    public async Task<IActionResult> FacebookLogin([FromBody] FacebookLoginRequest request)
    {
        var result = await _authService.FacebookLoginAsync(request.AccessToken);

        return Ok(result);
    }


    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
    {
        var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

        if (token == null || token.IsRevoked || token.ExpiresAt < DateTime.UtcNow)
            return Unauthorized("Invalid refresh token");

        var user = await _userRepository.GetByIdAsync(token.UserId);

        var newAccessToken = _tokenService.GenerateToken(user);

        return Ok(new AuthResponse
        {
            Token = newAccessToken,
            RefreshToken = refreshToken 
        });
    }


    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] string refreshToken)
    {
        await _refreshTokenRepository.RevokeAsync(refreshToken);

        return Ok("Logged out successfully ");
    }





    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        await _authService.ForgotPassword(dto.Email);
        return Ok("Reset code sent");
    }

    [HttpPost("verify-reset-code")]
    public async Task<IActionResult> VerifyCode([FromBody] VerifyResetCodeDto dto)
    {
        var user = await _userRepository.GetByResetTokenAsync(dto.Code);

        if (user == null || user.ResetTokenExpiry < DateTime.UtcNow)
            return BadRequest("Invalid or expired code");

        return Ok(new
        {
            resetToken = user.ResetToken
        });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        if (dto.NewPassword != dto.ConfirmPassword)
            return BadRequest("Passwords do not match");

        var user = await _userRepository
               .GetByResetTokenAsync(dto.ResetToken); 
        if (user == null || user.ResetTokenExpiry < DateTime.UtcNow)
            return BadRequest("Invalid or expired token");

        user.PasswordHash = _passwordHasher.Hash(dto.NewPassword);

        user.ResetToken = null;
        user.ResetTokenExpiry = null;

        await _userRepository.UpdateAsync(user);

        return Ok("Password reset successful");
    }


}
