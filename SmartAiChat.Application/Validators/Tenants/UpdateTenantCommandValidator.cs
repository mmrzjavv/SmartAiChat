using FluentValidation;
using SmartAiChat.Application.Commands.Tenants;

namespace SmartAiChat.Application.Validators.Tenants;

public class UpdateTenantCommandValidator : AbstractValidator<UpdateTenantCommand>
{
    public UpdateTenantCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Domain).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
