using CodeCraft.Application.DTOs.CoursesDTOs;
using FluentValidation;

namespace CodeCraft.Application.Validators;

public class CreateCourseRequestValidator : AbstractValidator<CreateCourseRequest>
{
    public CreateCourseRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Course title is required")
            .MaximumLength(200).WithMessage("Course title cannot exceed 200 characters");

        RuleFor(x => x.TrackId)
            .GreaterThan(0).WithMessage("Valid TrackId is required");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0).WithMessage("Order must be greater than or equal to 0");
    }
}
