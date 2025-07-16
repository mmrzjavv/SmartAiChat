using SmartAiChat.Shared.Enums;

namespace SmartAiChat.Application.DTOs;

public class ChatSessionDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? OperatorId { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public ChatSessionStatus Status { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerIpAddress { get; set; }
    public string? Subject { get; set; }
    public string? Department { get; set; }
    public DateTime? OperatorJoinedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public string? EndReason { get; set; }
    public decimal? CustomerSatisfactionRating { get; set; }
    public string? CustomerFeedback { get; set; }
    public int MessageCount { get; set; }
    public int AiMessageCount { get; set; }
    public List<string> Tags { get; set; } = new();
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public UserDto? Customer { get; set; }
    public UserDto? Operator { get; set; }
} 