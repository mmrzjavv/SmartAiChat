using MediatR;
using SmartAiChat.Application.DTOs;

namespace SmartAiChat.Application.Queries.Tenants;

public class GetTenantByIdQuery : IRequest<TenantDto>
{
    public Guid Id { get; set; }
}
