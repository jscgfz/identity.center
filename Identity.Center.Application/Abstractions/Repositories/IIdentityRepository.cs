using Identity.Center.Application.Abstractions.Managers;

namespace Identity.Center.Application.Abstractions.Repositories;

public interface IIdentityRepository<TEntity> : IBaseDbRepository<TEntity>
  where TEntity : class
{
}
