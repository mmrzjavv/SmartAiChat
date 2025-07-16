using AutoMapper;
using MediatR;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Application.Queries.Users;
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
        var usersPage = await _unitOfWork.Users.GetPagedAsync(request.Pagination, cancellationToken);
        var userDtos = _mapper.Map<IEnumerable<UserDto>>(usersPage.Items);
        return PaginatedResponse<UserDto>.Create(userDtos.ToList(), usersPage.TotalCount, usersPage.Page, usersPage.PageSize);
    }
}
