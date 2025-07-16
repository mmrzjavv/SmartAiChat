using AutoMapper;
using MediatR;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Application.Queries.ChatSessions;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Application.Handlers.ChatSessions;

public class GetChatSessionHandler : IRequestHandler<GetChatSessionQuery, ApiResponse<ChatSessionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ITenantContext _tenantContext;

    public GetChatSessionHandler(IUnitOfWork unitOfWork, IMapper mapper, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _tenantContext = tenantContext;
    }

    public async Task<ApiResponse<ChatSessionDto>> Handle(GetChatSessionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var chatSession = await _unitOfWork.ChatSessions.GetByIdAsync(request.Id, _tenantContext.TenantId, cancellationToken);

            if (chatSession == null)
            {
                return ApiResponse<ChatSessionDto>.Failure("Chat session not found");
            }

            var chatSessionDto = _mapper.Map<ChatSessionDto>(chatSession);
            
            // Load related data
            var customer = await _unitOfWork.Users.GetByIdAsync(chatSession.CustomerId, cancellationToken);
            if (customer != null)
            {
                chatSessionDto.Customer = _mapper.Map<UserDto>(customer);
            }

            if (chatSession.OperatorId.HasValue)
            {
                var operatorUser = await _unitOfWork.Users.GetByIdAsync(chatSession.OperatorId.Value, cancellationToken);
                if (operatorUser != null)
                {
                    chatSessionDto.Operator = _mapper.Map<UserDto>(operatorUser);
                }
            }

            return ApiResponse<ChatSessionDto>.Success(chatSessionDto, "Chat session retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<ChatSessionDto>.Failure($"Failed to retrieve chat session: {ex.Message}");
        }
    }
} 