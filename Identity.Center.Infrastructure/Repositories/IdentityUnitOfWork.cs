using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Persistence.Data.Core;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Infrastructure.Repositories;

internal sealed class IdentityUnitOfWork(IServiceProvider provider) : IIdentityUnitOfWork
{
  private readonly IdentityContext _context = provider.GetRequiredService<IdentityContext>();
  public IDbContextTransaction? CurrentTransaction => _context.Database.CurrentTransaction;

  public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    => _context.SaveChangesAsync(cancellationToken);
}
