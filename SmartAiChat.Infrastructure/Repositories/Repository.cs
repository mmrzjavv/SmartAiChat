using Microsoft.EntityFrameworkCore;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Persistence;
using SmartAiChat.Shared;
using SmartAiChat.Shared.Models;
using System.Linq.Expressions;

namespace SmartAiChat.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext Context;
    protected readonly DbSet<T> DbSet;
    private readonly ITenantContext? _tenantContext;

    public Repository(ApplicationDbContext context, ITenantContext? tenantContext = null)
    {
        Context = context;
        DbSet = context.Set<T>();
        _tenantContext = tenantContext;
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var entity = await DbSet.FindAsync(new object[] { id }, cancellationToken);
        
        // Check tenant ownership for entities that have TenantId
        if (entity != null && typeof(T).GetProperty("TenantId") != null)
        {
            var entityTenantId = (Guid?)typeof(T).GetProperty("TenantId")?.GetValue(entity);
            if (entityTenantId != tenantId)
                return null;
        }
        
        return entity;
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        if (typeof(T).GetProperty("TenantId") != null)
        {
            return await DbSet.Where(e => EF.Property<Guid>(e, "TenantId") == tenantId).ToListAsync(cancellationToken);
        }
        
        return await GetAllAsync(cancellationToken);
    }

    public virtual async Task<PaginatedResponse<T>> GetPagedAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        request.EnsureValidPagination();
        
        var query = DbSet.AsQueryable();
        
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            // Add basic search functionality - this would need to be customized per entity
            // For now, just return all results
        }
        
        var totalCount = await query.CountAsync(cancellationToken);
        
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            // Add sorting logic - simplified for now
            query = request.SortDescending 
                ? query.OrderByDescending(e => e.CreatedAt)
                : query.OrderBy(e => e.CreatedAt);
        }
        else
        {
            query = query.OrderByDescending(e => e.CreatedAt);
        }
        
        var items = await query
            .Skip(request.Skip)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
            
        return PaginatedResponse<T>.Create(items, totalCount, request.Page, request.PageSize);
    }

    public virtual async Task<PaginatedResponse<T>> GetPagedAsync(PaginationRequest request, Guid tenantId, CancellationToken cancellationToken = default)
    {
        request.EnsureValidPagination();
        
        var query = DbSet.AsQueryable();
        
        if (typeof(T).GetProperty("TenantId") != null)
        {
            query = query.Where(e => EF.Property<Guid>(e, "TenantId") == tenantId);
        }
        
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            // Add basic search functionality - this would need to be customized per entity
        }
        
        var totalCount = await query.CountAsync(cancellationToken);
        
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            query = request.SortDescending 
                ? query.OrderByDescending(e => e.CreatedAt)
                : query.OrderBy(e => e.CreatedAt);
        }
        else
        {
            query = query.OrderByDescending(e => e.CreatedAt);
        }
        
        var items = await query
            .Skip(request.Skip)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
            
        return PaginatedResponse<T>.Create(items, totalCount, request.Page, request.PageSize);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        if (typeof(T).GetProperty("TenantId") != null)
        {
            return await DbSet.CountAsync(e => EF.Property<Guid>(e, "TenantId") == tenantId, cancellationToken);
        }
        
        return await CountAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        var entry = await DbSet.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        DbSet.UpdateRange(entities);
        return Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            DbSet.Remove(entity);
        }
    }

    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        DbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    public virtual async Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }
    }

    public virtual Task SoftDeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        return Task.CompletedTask;
    }
} 