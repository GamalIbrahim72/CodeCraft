using System.ComponentModel.DataAnnotations;

namespace CodeCraft.Application.DTOs.AuthDTOs;

public class RegisterRequest
{
    [Required]
    [MinLength(2)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MinLength(2)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone(ErrorMessage = "Invalid phone number")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
        ErrorMessage = "Password must contain uppercase, lowercase, number, and special character"
    )]
    public string Password { get; set; } = string.Empty;
}