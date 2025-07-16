using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Shared.Constants;
using SmartAiChat.Shared.Models;

namespace SmartAiChat.Infrastructure.Services;

public class TenantContextService : ITenantContext
{
    private TenantContext? _currentContext;

    public TenantContext? Current => _currentContext;

    public Guid TenantId => _currentContext?.TenantId ?? Guid.Empty;

    public Guid UserId => _currentContext?.UserId ?? Guid.Empty;

    public string UserRole => _currentContext?.UserRole ?? string.Empty;

    public void SetContext(TenantContext context)
    {
        _currentContext = context;
    }

    public void ClearContext()
    {
        _currentContext = null;
    }

    public bool HasPermission(string permission)
    {
        return _currentContext?.Permissions.Contains(permission) ?? false;
    }

    public bool IsSuperAdmin()
    {
        return UserRole == SystemConstants.Roles.SuperAdmin;
    }

    public bool IsTenantAdmin()
    {
        return UserRole == SystemConstants.Roles.TenantAdmin;
    }

    public bool IsOperator()
    {
        return UserRole == SystemConstants.Roles.Operator;
    }

    public bool IsCustomer()
    {
        return UserRole == SystemConstants.Roles.Customer;
    }
} 