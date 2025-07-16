using Microsoft.EntityFrameworkCore;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Persistence;
using SmartAiChat.Infrastructure.Repositories.Search;
using SmartAiChat.Shared;
using SmartAiChat.Shared.Models;
using System.Linq.Expressions;

namespace SmartAiChat.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;
    private readonly ITenantContext? _tenantContext;

    public Repository(ApplicationDbContext context, ITenantContext? tenantContext = null)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _tenantContext = tenantContext;
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        
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
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        if (typeof(T).GetProperty("TenantId") != null)
        {
            return await _dbSet.Where(e => EF.Property<Guid>(e, "TenantId") == tenantId).ToListAsync(cancellationToken);
        }
        
        return await GetAllAsync(cancellationToken);
    }

    public virtual async Task<PaginatedResponse<T>> GetPagedAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        request.EnsureValidPagination();
        
        var query = _dbSet.AsQueryable();
        
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchExpression = SearchExpressionBuilder.Build<T>(request.SearchTerm);
            query = query.Where(searchExpression);
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
        
        var query = _dbSet.AsQueryable();
        
        if (typeof(T).GetProperty("TenantId") != null)
        {
            query = query.Where(e => EF.Property<Guid>(e, "TenantId") == tenantId);
        }
        
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchExpression = SearchExpressionBuilder.Build<T>(request.SearchTerm);
            query = query.Where(searchExpression);
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
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        if (typeof(T).GetProperty("TenantId") != null)
        {
            return await _dbSet.CountAsync(e => EF.Property<Guid>(e, "TenantId") == tenantId, cancellationToken);
        }
        
        return await CountAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        var entry = await _dbSet.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.UpdateRange(entities);
        return Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.RemoveRange(entities);
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