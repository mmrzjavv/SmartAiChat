using MediatR;
using SmartAiChat.Application.DTOs;

namespace SmartAiChat.Application.Commands.Tenants;

public class CreateTenantCommand : IRequest<TenantDto>
{
    public string Name { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
