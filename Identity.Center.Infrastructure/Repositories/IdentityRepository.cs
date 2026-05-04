using Identity.Center.Persistence.Data.Core;

namespace Identity.Center.Infrastructure.Repositories;

internal sealed class IdentityRepository<TEntity>(IServiceProvider provider) : BaseDbRepository<IdentityContext, TEntity>(provider)
  where TEntity : class
{ }
