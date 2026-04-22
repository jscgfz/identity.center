namespace Identity.Center.Application.Abstractions.Managers;

public interface IBaseDbRepository<TEntity>
  where TEntity : class
{
  IQueryable<TEntity> Data { get; }
}
