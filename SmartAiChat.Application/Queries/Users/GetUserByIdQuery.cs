using MediatR;
using SmartAiChat.Application.DTOs;

namespace SmartAiChat.Application.Queries.Users;

public class GetUserByIdQuery : IRequest<UserDto>
{
    public Guid Id { get; set; }
}
