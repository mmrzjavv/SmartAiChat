using MediatR;

namespace SmartAiChat.Application.Commands.Users;

public class DeleteUserCommand : IRequest
{
    public Guid Id { get; set; }
}
