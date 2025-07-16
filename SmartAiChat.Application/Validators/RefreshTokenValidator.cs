using FluentValidation;
using SmartAiChat.Application.Commands.Authentication;

namespace SmartAiChat.Application.Validators;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
} 