using Identity.Center.Application.Abstractions.Managers;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Persistence.Data.Core;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Infrastructure.Repositories;

internal sealed class IdentityRepository<TEntity>(IServiceProvider provider) : BaseDbRepository<IdentityContext, TEntity>(provider)
  where TEntity : class
{ }
