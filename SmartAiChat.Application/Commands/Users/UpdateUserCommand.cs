using MediatR;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Shared.Enums;

namespace SmartAiChat.Application.Commands.Users;

public class UpdateUserCommand : IRequest<UserDto>
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
}
