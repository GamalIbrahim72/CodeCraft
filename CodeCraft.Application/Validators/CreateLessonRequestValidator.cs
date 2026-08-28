using CodeCraft.Application.DTOs.Lessons;
using FluentValidation;

namespace CodeCraft.Application.Validators;

public class CreateLessonRequestValidator : AbstractValidator<CreateLessonRequest>
{
    public CreateLessonRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Lesson title is required")
            .MaximumLength(200).WithMessage("Lesson title cannot exceed 200 characters");

        RuleFor(x => x.CourseId)
            .GreaterThan(0).WithMessage("Valid CourseId is required");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0).WithMessage("Order must be greater than or equal to 0");
    }
}
