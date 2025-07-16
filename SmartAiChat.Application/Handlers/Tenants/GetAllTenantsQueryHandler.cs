using AutoMapper;
using MediatR;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Application.Queries.Tenants;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Application.Handlers.Tenants;

public class GetAllTenantsQueryHandler : IRequestHandler<GetAllTenantsQuery, PaginatedResponse<TenantDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllTenantsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

public async Task<PaginatedResponse<TenantDto>> Handle(GetAllTenantsQuery request, CancellationToken cancellationToken)
    {
        var tenants = await _unitOfWork.Tenants.GetPagedAsync(request.Pagination, cancellationToken);
        var tenantDtos = _mapper.Map<IEnumerable<TenantDto>>(tenants.Items);
        return PaginatedResponse<TenantDto>.Create(tenantDtos.ToList(), tenants.TotalCount, tenants.Page, tenants.PageSize);
    }
}
