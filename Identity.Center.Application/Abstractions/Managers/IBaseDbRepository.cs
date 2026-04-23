using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Identity.Center.Application.Abstractions.Managers;

public interface IBaseDbRepository<TEntity>
  where TEntity : class
{
  IQueryable<TEntity> Data { get; }
  ValueTask<EntityEntry<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
  Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
  EntityEntry<TEntity> Remove(TEntity entity);
  void RemoveRange(IEnumerable<TEntity> entities);
  EntityEntry<TEntity> Update(TEntity entity);
  void UpdateRange(IEnumerable<TEntity> entities);
}
