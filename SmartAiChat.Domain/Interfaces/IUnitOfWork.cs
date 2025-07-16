using SmartAiChat.Domain.Entities;

namespace SmartAiChat.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Tenant> Tenants { get; }
    IRepository<TenantPlan> TenantPlans { get; }
    IRepository<TenantSubscription> TenantSubscriptions { get; }
    IRepository<User> Users { get; }
    IRepository<ChatSession> ChatSessions { get; }
    IRepository<ChatMessage> ChatMessages { get; }
    IRepository<OperatorActivity> OperatorActivities { get; }
    IRepository<AiConfiguration> AiConfigurations { get; }
    IRepository<FaqEntry> FaqEntries { get; }
    IRepository<AiTrainingFile> AiTrainingFiles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
} 