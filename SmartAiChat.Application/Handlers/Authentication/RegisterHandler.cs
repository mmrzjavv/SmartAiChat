using AutoMapper;
using MediatR;
using SmartAiChat.Application.Commands.Authentication;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Application.Handlers.Authentication;

public class RegisterHandler : IRequestHandler<RegisterCommand, ApiResponse<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RegisterHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<UserDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(
                u => u.Email == request.Email && u.TenantId == request.TenantId,
                cancellationToken);

            if (existingUser != null)
            {
                return ApiResponse<UserDto>.Failure("User with this email already exists");
            }

            // Validate tenant exists
            var tenant = await _unitOfWork.Tenants.GetByIdAsync(request.TenantId, cancellationToken);
            if (tenant == null || !tenant.IsActive)
            {
                return ApiResponse<UserDto>.Failure("Invalid tenant");
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Create new user
            var user = new User
            {
                TenantId = request.TenantId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = request.Role,
                IsActive = true,
                EmailConfirmed = false
            };

            user = await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var userDto = _mapper.Map<UserDto>(user);
            return ApiResponse<UserDto>.Success(userDto, "User registered successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<UserDto>.Failure($"Registration failed: {ex.Message}");
        }
    }
} 