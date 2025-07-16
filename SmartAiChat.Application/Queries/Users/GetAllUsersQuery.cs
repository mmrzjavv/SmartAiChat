using MediatR;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Application.Queries.Users;

public class GetAllUsersQuery : IRequest<PaginatedResponse<UserDto>>
{
    public PaginationRequest Pagination { get; set; } = new();
}
