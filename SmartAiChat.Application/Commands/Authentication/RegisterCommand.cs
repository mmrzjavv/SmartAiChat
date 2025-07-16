using MediatR;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Shared.Enums;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Application.Commands.Authentication;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    Guid TenantId,
    UserRole Role = UserRole.Customer
) : IRequest<ApiResponse<UserDto>>; 