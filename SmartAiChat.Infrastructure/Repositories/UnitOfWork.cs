using Microsoft.EntityFrameworkCore.Storage;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Infrastructure.Repositories;
using SmartAiChat.Persistence;

namespace SmartAiChat.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantContext? _tenantContext;
    private IDbContextTransaction? _transaction;

    private IRepository<Tenant>? _tenants;
    private IRepository<TenantPlan>? _tenantPlans;
    private IRepository<TenantSubscription>? _tenantSubscriptions;
    private IRepository<User>? _users;
    private IRepository<ChatSession>? _chatSessions;
    private IRepository<ChatMessage>? _chatMessages;
    private IRepository<OperatorActivity>? _operatorActivities;
    private IRepository<AiConfiguration>? _aiConfigurations;
    private IRepository<FaqEntry>? _faqEntries;
    private IRepository<AiTrainingFile>? _aiTrainingFiles;

    public UnitOfWork(ApplicationDbContext context, ITenantContext? tenantContext = null)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public IRepository<Tenant> Tenants => _tenants ??= new Repository<Tenant>(_context, _tenantContext);
    public IRepository<TenantPlan> TenantPlans => _tenantPlans ??= new Repository<TenantPlan>(_context, _tenantContext);
    public IRepository<TenantSubscription> TenantSubscriptions => _tenantSubscriptions ??= new Repository<TenantSubscription>(_context, _tenantContext);
    public IRepository<User> Users => _users ??= new Repository<User>(_context, _tenantContext);
    public IRepository<ChatSession> ChatSessions => _chatSessions ??= new Repository<ChatSession>(_context, _tenantContext);
    public IRepository<ChatMessage> ChatMessages => _chatMessages ??= new Repository<ChatMessage>(_context, _tenantContext);
    public IRepository<OperatorActivity> OperatorActivities => _operatorActivities ??= new Repository<OperatorActivity>(_context, _tenantContext);
    public IRepository<AiConfiguration> AiConfigurations => _aiConfigurations ??= new Repository<AiConfiguration>(_context, _tenantContext);
    public IRepository<FaqEntry> FaqEntries => _faqEntries ??= new Repository<FaqEntry>(_context, _tenantContext);
    public IRepository<AiTrainingFile> AiTrainingFiles => _aiTrainingFiles ??= new Repository<AiTrainingFile>(_context, _tenantContext);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
} 