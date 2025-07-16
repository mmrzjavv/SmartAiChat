using FluentValidation;
using SmartAiChat.Application.Commands.ChatMessages;

namespace SmartAiChat.Application.Validators;

public class SendMessageValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageValidator()
    {
        RuleFor(x => x.ChatSessionId)
            .NotEmpty()
            .WithMessage("Chat session ID is required");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Message content is required")
            .MaximumLength(10000)
            .WithMessage("Message content cannot exceed 10,000 characters");

        RuleFor(x => x.MessageType)
            .IsInEnum()
            .WithMessage("Invalid message type");

        RuleFor(x => x.AttachmentName)
            .MaximumLength(255)
            .WithMessage("Attachment name cannot exceed 255 characters")
            .When(x => !string.IsNullOrEmpty(x.AttachmentName));

        RuleFor(x => x.AttachmentUrl)
            .MaximumLength(500)
            .WithMessage("Attachment URL cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.AttachmentUrl));

        RuleFor(x => x.AttachmentMimeType)
            .MaximumLength(100)
            .WithMessage("Attachment MIME type cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.AttachmentMimeType));

        RuleFor(x => x.AttachmentSize)
            .GreaterThan(0)
            .WithMessage("Attachment size must be greater than 0")
            .LessThanOrEqualTo(50 * 1024 * 1024) // 50 MB
            .WithMessage("Attachment size cannot exceed 50 MB")
            .When(x => x.AttachmentSize.HasValue);
    }
} 