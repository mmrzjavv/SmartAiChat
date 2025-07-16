using MediatR;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Application.Commands.Authentication;

public record LoginCommand(
    string Email,
    string Password,
    Guid? TenantId = null
) : IRequest<ApiResponse<LoginResponseDto>>; 