using MediatR;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Application.Queries.Tenants;

public class GetAllTenantsQuery : IRequest<PaginatedResponse<TenantDto>>
{
    public PaginationRequest Pagination { get; set; } = new();
}
