using CodeCraft.Application.DTOs.Community;
using FluentValidation;

namespace CodeCraft.Application.Validators;

public class SendTrackMessageDtoValidator : AbstractValidator<SendTrackMessageDto>
{
    public SendTrackMessageDtoValidator()
    {
        RuleFor(x => x.TrackId)
            .GreaterThan(0).WithMessage("Valid TrackId is required");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Message content cannot be empty")
            .MaximumLength(2000).WithMessage("Message content cannot exceed 2000 characters");
    }
}
