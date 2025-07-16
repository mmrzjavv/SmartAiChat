namespace SmartAiChat.Shared.Constants;

public static class SystemConstants
{
    public static class Roles
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string TenantAdmin = "TenantAdmin";
        public const string Operator = "Operator";
        public const string Customer = "Customer";
    }

    public static class Policies
    {
        public const string SuperAdminOnly = "SuperAdminOnly";
        public const string TenantAdminOrAbove = "TenantAdminOrAbove";
        public const string OperatorOrAbove = "OperatorOrAbove";
        public const string CustomerOrAbove = "CustomerOrAbove";
    }

    public static class ClaimTypes
    {
        public const string TenantId = "tenant_id";
        public const string UserId = "user_id";
        public const string Role = "role";
        public const string Permissions = "permissions";
    }

    public static class DefaultSettings
    {
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 100;
        public const int DefaultAiWordLimit = 500;
        public const int DefaultMaxDailyMessages = 1000;
        public const int DefaultMaxConcurrentSessions = 50;
    }

    public static class Cache
    {
        public const int DefaultCacheExpirationMinutes = 30;
        public const string TenantConfigPrefix = "tenant_config_";
        public const string UserPermissionsPrefix = "user_permissions_";
    }
} 