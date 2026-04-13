using CodeCraft.Application.DTOs.AuthDTOs;
using CodeCraft.Application.DTOs.Password;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.Interfaces.Services;
public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> GoogleLoginAsync(string idToken);
    Task<AuthResponse> FacebookLoginAsync(string accessToken);

    Task ForgotPassword(string email);
    Task ResetPassword(ResetPasswordDto dto);

}

