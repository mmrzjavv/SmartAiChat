using MediatR;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Application.Commands.Authentication;

public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<ApiResponse<LoginResponseDto>>; 