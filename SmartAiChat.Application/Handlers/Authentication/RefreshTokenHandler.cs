using AutoMapper;
using MediatR;
using SmartAiChat.Application.Commands.Authentication;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Application.Handlers.Authentication;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<LoginResponseDto>>
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RefreshTokenHandler(IJwtTokenService jwtTokenService, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<LoginResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find user with the refresh token
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(
                u => u.RefreshToken == request.RefreshToken && 
                     u.RefreshTokenExpiresAt > DateTime.UtcNow &&
                     u.IsActive,
                cancellationToken);

            if (user == null)
            {
                return ApiResponse<LoginResponseDto>.Failure("Invalid or expired refresh token");
            }

            // Generate new tokens
            var tokens = await _jwtTokenService.GenerateTokensAsync(user, cancellationToken);

            // Map to response DTO
            var userDto = _mapper.Map<UserDto>(user);
            var response = new LoginResponseDto
            {
                User = userDto,
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                AccessTokenExpires = tokens.AccessTokenExpires,
                RefreshTokenExpires = tokens.RefreshTokenExpires,
                TokenType = tokens.TokenType
            };

            return ApiResponse<LoginResponseDto>.Success(response, "Token refreshed successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<LoginResponseDto>.Failure($"Token refresh failed: {ex.Message}");
        }
    }
} 