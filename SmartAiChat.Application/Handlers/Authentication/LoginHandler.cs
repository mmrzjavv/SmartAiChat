using AutoMapper;
using MediatR;
using SmartAiChat.Application.Commands.Authentication;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Shared.Models;
using BCrypt.Net;

namespace SmartAiChat.Application.Handlers.Authentication;

public class LoginHandler : IRequestHandler<LoginCommand, ApiResponse<LoginResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;

    public LoginHandler(IUnitOfWork unitOfWork, IJwtTokenService jwtTokenService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find user by email and optionally tenant
            var user = request.TenantId.HasValue
                ? await _unitOfWork.Users.FirstOrDefaultAsync(
                    u => u.Email == request.Email && u.TenantId == request.TenantId.Value && u.IsActive,
                    cancellationToken)
                : await _unitOfWork.Users.FirstOrDefaultAsync(
                    u => u.Email == request.Email && u.IsActive,
                    cancellationToken);

            if (user == null)
            {
                return ApiResponse<LoginResponseDto>.Failure("Invalid email or password");
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return ApiResponse<LoginResponseDto>.Failure("Invalid email or password");
            }

            // Generate tokens
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

            return ApiResponse<LoginResponseDto>.Success(response, "Login successful");
        }
        catch (Exception ex)
        {
            return ApiResponse<LoginResponseDto>.Failure($"Login failed: {ex.Message}");
        }
    }
} 