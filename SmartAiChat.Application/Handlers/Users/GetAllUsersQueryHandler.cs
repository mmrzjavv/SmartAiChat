using AutoMapper;
using MediatR;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Application.Queries.Users;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Application.Handlers.Users;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PaginatedResponse<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var (users, totalCount) = await _unitOfWork.Repository<User>().GetAllAsync(
            request.Pagination.Page,
            request.Pagination.PageSize,
            u => u.TenantId == _unitOfWork.TenantContext.GetTenantId());

        var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

        return new PaginatedResponse<UserDto>(userDtos, totalCount, request.Pagination.Page, request.Pagination.PageSize);
    }
}
