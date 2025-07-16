using Microsoft.EntityFrameworkCore;
using SmartAiChat.Domain.Entities;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Shared;
using System.Reflection;

namespace SmartAiChat.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly ITenantContext? _tenantContext;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantContext tenantContext) 
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantPlan> TenantPlans { get; set; }
    public DbSet<TenantSubscription> TenantSubscriptions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ChatSession> ChatSessions { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<OperatorActivity> OperatorActivities { get; set; }
    public DbSet<AiConfiguration> AiConfigurations { get; set; }
    public DbSet<FaqEntry> FaqEntries { get; set; }
    public DbSet<AiTrainingFile> AiTrainingFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Apply global tenant filter for multi-tenancy (except for Tenant and TenantPlan entities)
        if (_tenantContext != null)
        {
            // Apply tenant filter to all entities that implement ITenantEntity
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType) && 
                    entityType.ClrType.GetProperty("TenantId") != null &&
                    entityType.ClrType != typeof(Tenant) &&
                    entityType.ClrType != typeof(TenantPlan))
                {
                    var method = typeof(ApplicationDbContext)
                        .GetMethod(nameof(SetGlobalTenantFilter), BindingFlags.NonPublic | BindingFlags.Instance)?
                        .MakeGenericMethod(entityType.ClrType);
                    
                    method?.Invoke(this, new object[] { modelBuilder });
                }
            }
        }
    }

    private void SetGlobalTenantFilter<T>(ModelBuilder modelBuilder) where T : class
    {
        if (_tenantContext != null)
        {
            modelBuilder.Entity<T>().HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == _tenantContext.TenantId);
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Set audit fields
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    
                    // Set TenantId for new entities (except Tenant and TenantPlan)
                    if (_tenantContext != null && 
                        entry.Entity.GetType().GetProperty("TenantId") != null &&
                        entry.Entity.GetType() != typeof(Tenant) &&
                        entry.Entity.GetType() != typeof(TenantPlan))
                    {
                        entry.Entity.GetType().GetProperty("TenantId")?.SetValue(entry.Entity, _tenantContext.TenantId);
                    }
                    break;
                    
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Property(e => e.CreatedAt).IsModified = false; // Prevent modification of CreatedAt
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
} 