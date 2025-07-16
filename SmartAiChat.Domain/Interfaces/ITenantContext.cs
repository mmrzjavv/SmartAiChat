using SmartAiChat.Shared.Models;

namespace SmartAiChat.Domain.Interfaces;

public interface ITenantContext
{
    TenantContext? Current { get; }
    Guid TenantId { get; }
    Guid UserId { get; }
    string UserRole { get; }
    void SetContext(TenantContext context);
    void ClearContext();
    bool HasPermission(string permission);
    bool IsSuperAdmin();
    bool IsTenantAdmin();
    bool IsOperator();
    bool IsCustomer();
} 