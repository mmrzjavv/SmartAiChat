using AutoMapper;
using MediatR;
using SmartAiChat.Application.Commands.ChatSessions;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Shared.Enums;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Application.Handlers.ChatSessions;

public class StartChatSessionHandler : IRequestHandler<StartChatSessionCommand, ApiResponse<ChatSessionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ITenantContext _tenantContext;

    public StartChatSessionHandler(IUnitOfWork unitOfWork, IMapper mapper, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _tenantContext = tenantContext;
    }

    public async Task<ApiResponse<ChatSessionDto>> Handle(StartChatSessionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find or create customer user
            var customer = await _unitOfWork.Users.FirstOrDefaultAsync(
                u => u.Email == request.CustomerEmail && u.TenantId == _tenantContext.TenantId,
                cancellationToken);

            if (customer == null)
            {
                // Create new customer user
                customer = new User
                {
                    TenantId = _tenantContext.TenantId,
                    FirstName = request.CustomerName.Split(' ').FirstOrDefault() ?? request.CustomerName,
                    LastName = request.CustomerName.Split(' ').Skip(1).FirstOrDefault() ?? "",
                    Email = request.CustomerEmail,
                    Role = UserRole.Customer,
                    PasswordHash = "temp", // Customer users don't need passwords for chat
                    IsActive = true,
                    EmailConfirmed = false
                };

                customer = await _unitOfWork.Users.AddAsync(customer, cancellationToken);
            }

            // Create new chat session
            var chatSession = new ChatSession
            {
                TenantId = _tenantContext.TenantId,
                CustomerId = customer.Id,
                SessionId = Guid.NewGuid().ToString("N")[..8], // Short session ID
                Status = ChatSessionStatus.Active,
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                CustomerIpAddress = request.CustomerIpAddress,
                CustomerUserAgent = request.CustomerUserAgent,
                Subject = request.Subject,
                Department = request.Department
            };

            chatSession = await _unitOfWork.ChatSessions.AddAsync(chatSession, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var chatSessionDto = _mapper.Map<ChatSessionDto>(chatSession);
            chatSessionDto.Customer = _mapper.Map<UserDto>(customer);

            return ApiResponse<ChatSessionDto>.Success(chatSessionDto, "Chat session started successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<ChatSessionDto>.Failure($"Failed to start chat session: {ex.Message}");
        }
    }
} 