using MediatR;

namespace SmartAiChat.Application.Commands.Users;

public class DeleteUserCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
