using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Persistence.Data.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Infrastructure.Repositories;

internal sealed class IdentityRepository<TEntity>(IServiceProvider provider) : IIdentityRepository<TEntity>
  where TEntity : class
{
  private readonly IdentityContext _context = provider.GetRequiredService<IdentityContext>();

  public IQueryable<TEntity> Data => _context.Set<TEntity>();
}
