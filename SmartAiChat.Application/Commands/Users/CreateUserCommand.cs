using MediatR;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Shared.Enums;

namespace SmartAiChat.Application.Commands.Users;

public class CreateUserCommand : IRequest<UserDto>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; }
    public Guid TenantId { get; set; }
}
