using MediatR;

namespace SmartAiChat.Application.Commands.Tenants;

public class DeleteTenantCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
