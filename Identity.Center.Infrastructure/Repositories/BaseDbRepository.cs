using Identity.Center.Application.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Infrastructure.Repositories;

public abstract class BaseDbRepository<TContext, TEntity>(IServiceProvider provider) : IIdentityRepository<TEntity>
  where TEntity : class
  where TContext : DbContext
{
  protected TContext Context => _context;
  private readonly TContext _context = provider.GetRequiredService<TContext>();
  public IQueryable<TEntity> Data => _context.Set<TEntity>();

  public ValueTask<EntityEntry<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    => _context.AddAsync(entity, cancellationToken);

  public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    => _context.AddRangeAsync(entities, cancellationToken);

  public EntityEntry<TEntity> Remove(TEntity entity)
    => _context.Remove(entity);

  public void RemoveRange(IEnumerable<TEntity> entities)
    => _context.RemoveRange(entities);

  public EntityEntry<TEntity> Update(TEntity entity)
    => _context.Update(entity);

  public void UpdateRange(IEnumerable<TEntity> entities)
    => _context.UpdateRange(entities);
}
