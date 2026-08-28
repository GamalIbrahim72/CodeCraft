using CodeCraft.Application.DTOs.AuthDTOs;
using CodeCraft.Application.DTOs.Community;
using CodeCraft.Application.DTOs.CoursesDTOs;
using CodeCraft.Application.DTOs.Lessons;
using CodeCraft.Application.DTOs.Password;
using CodeCraft.Application.Validators;
using FluentAssertions;
using Xunit;

namespace CodeCraft.Tests.Validators;

public class ValidationTests
{
    [Fact]
    public void RegisterRequestValidator_WhenValid_PassesValidation()
    {
        var validator = new RegisterRequestValidator();
        var request = new RegisterRequest
        {
            FirstName = "Ali",
            LastName = "Hassan",
            Email = "ali@example.com",
            Phone = "01012345678",
            Password = "Password@123",
            DateOfBirth = new DateTime(2000, 1, 1)
        };

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Invalid", "Empty first name")]
    [InlineData("Ali", "invalid-email", "Invalid email")]
    [InlineData("Ali", "ali@test.com", "weak")] // Password too weak/short
    public void RegisterRequestValidator_WhenInvalid_FailsValidation(string firstName, string email, string password)
    {
        var validator = new RegisterRequestValidator();
        var request = new RegisterRequest
        {
            FirstName = firstName,
            LastName = "Hassan",
            Email = email,
            Phone = "01012345678",
            Password = password,
            DateOfBirth = new DateTime(2000, 1, 1)
        };

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void LoginRequestValidator_WhenValid_PassesValidation()
    {
        var validator = new LoginRequestValidator();
        var request = new LoginRequest
        {
            Email = "user@test.com",
            Password = "ValidPassword1!"
        };

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ResetPasswordDtoValidator_WhenPasswordsMismatch_FailsValidation()
    {
        var validator = new ResetPasswordDtoValidator();
        var request = new ResetPasswordDto
        {
            ResetToken = "token123",
            NewPassword = "Password123!",
            ConfirmPassword = "Mismatch123!"
        };

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ConfirmPassword");
    }

    [Fact]
    public void CreateCourseRequestValidator_WhenTitleEmpty_FailsValidation()
    {
        var validator = new CreateCourseRequestValidator();
        var request = new CreateCourseRequest
        {
            Title = "",
            TrackId = 1,
            Order = 1
        };

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void CreateLessonRequestValidator_WhenCourseIdInvalid_FailsValidation()
    {
        var validator = new CreateLessonRequestValidator();
        var request = new CreateLessonRequest
        {
            Title = "Introduction",
            CourseId = 0,
            Order = 1
        };

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CourseId");
    }

    [Fact]
    public void SendTrackMessageDtoValidator_WhenContentEmpty_FailsValidation()
    {
        var validator = new SendTrackMessageDtoValidator();
        var request = new SendTrackMessageDto
        {
            TrackId = 1,
            Content = ""
        };

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Content");
    }
}
