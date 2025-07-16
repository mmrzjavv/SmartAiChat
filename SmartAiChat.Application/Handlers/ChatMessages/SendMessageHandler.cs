using AutoMapper;
using MediatR;
using SmartAiChat.Application.Commands.ChatMessages;
using SmartAiChat.Application.DTOs;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Shared.Enums;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Application.Handlers.ChatMessages;

public class SendMessageHandler : IRequestHandler<SendMessageCommand, ApiResponse<ChatMessageDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ITenantContext _tenantContext;
    private readonly IAiService _aiService;

    public SendMessageHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ITenantContext tenantContext,
        IAiService aiService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _tenantContext = tenantContext;
        _aiService = aiService;
    }

    public async Task<ApiResponse<ChatMessageDto>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get the chat session
            var chatSession = await _unitOfWork.ChatSessions.GetByIdAsync(request.ChatSessionId, _tenantContext.TenantId, cancellationToken);
            if (chatSession == null)
            {
                return ApiResponse<ChatMessageDto>.Failure("Chat session not found");
            }

            // Create the customer message
            var customerMessage = new ChatMessage
            {
                TenantId = _tenantContext.TenantId,
                ChatSessionId = request.ChatSessionId,
                UserId = chatSession.CustomerId,
                MessageType = request.MessageType,
                Content = request.Content,
                IsFromAi = false,
                AttachmentUrl = request.AttachmentUrl,
                AttachmentName = request.AttachmentName,
                AttachmentMimeType = request.AttachmentMimeType,
                AttachmentSize = request.AttachmentSize,
                WordCount = request.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length
            };

            customerMessage = await _unitOfWork.ChatMessages.AddAsync(customerMessage, cancellationToken);

            // Update session message count
            chatSession.MessageCount++;
            await _unitOfWork.ChatSessions.UpdateAsync(chatSession, cancellationToken);

            // Generate AI response if needed
            ChatMessage? aiResponse = null;
            if (request.MessageType == ChatMessageType.CustomerMessage && chatSession.Status == ChatSessionStatus.Active)
            {
                var aiConfig = await _unitOfWork.AiConfigurations.FirstOrDefaultAsync(
                    ac => ac.TenantId == _tenantContext.TenantId, cancellationToken);

                if (aiConfig?.IsEnabled == true)
                {
                    // Get conversation history
                    var recentMessages = await _unitOfWork.ChatMessages.FindAsync(
                        m => m.ChatSessionId == request.ChatSessionId,
                        cancellationToken);

                    var conversationHistory = recentMessages
                        .OrderBy(m => m.CreatedAt)
                        .TakeLast(aiConfig.ContextHistoryLimit)
                        .ToList();

                    // Generate AI response
                    var aiResponseContent = await _aiService.GenerateResponseAsync(
                        request.Content, aiConfig, conversationHistory, cancellationToken);

                    // Create AI message
                    aiResponse = new ChatMessage
                    {
                        TenantId = _tenantContext.TenantId,
                        ChatSessionId = request.ChatSessionId,
                        MessageType = ChatMessageType.AiResponse,
                        Content = aiResponseContent,
                        IsFromAi = true,
                        AiModel = aiConfig.ModelName,
                        WordCount = aiResponseContent.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length
                    };

                    aiResponse = await _unitOfWork.ChatMessages.AddAsync(aiResponse, cancellationToken);
                    
                    // Update session AI message count
                    chatSession.AiMessageCount++;
                    await _unitOfWork.ChatSessions.UpdateAsync(chatSession, cancellationToken);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = _mapper.Map<ChatMessageDto>(customerMessage);
            return ApiResponse<ChatMessageDto>.Success(result, "Message sent successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<ChatMessageDto>.Failure($"Failed to send message: {ex.Message}");
        }
    }
} 