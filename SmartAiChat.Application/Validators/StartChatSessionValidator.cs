using FluentValidation;
using SmartAiChat.Application.Commands.ChatSessions;

namespace SmartAiChat.Application.Validators;

public class StartChatSessionValidator : AbstractValidator<StartChatSessionCommand>
{
    public StartChatSessionValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithMessage("Customer name is required")
            .MaximumLength(200)
            .WithMessage("Customer name cannot exceed 200 characters");

        RuleFor(x => x.CustomerEmail)
            .NotEmpty()
            .WithMessage("Customer email is required")
            .EmailAddress()
            .WithMessage("Please provide a valid email address")
            .MaximumLength(200)
            .WithMessage("Email cannot exceed 200 characters");

        RuleFor(x => x.Subject)
            .MaximumLength(200)
            .WithMessage("Subject cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Subject));

        RuleFor(x => x.Department)
            .MaximumLength(100)
            .WithMessage("Department cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Department));
    }
} 