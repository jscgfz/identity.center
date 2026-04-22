using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Apps.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Entities.Core.Builds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Features.Apps.Queries.GetApps;

internal sealed class GetAppsQueryHandler(IServiceProvider provider) : IQueryHandler<GetAppsQuery, IEnumerable<AppDto>>
{
  private readonly IIdentityRepository<App> _repo = provider.GetRequiredService<IIdentityRepository<App>>();
  public async Task<Result<IEnumerable<AppDto>>> Handle(GetAppsQuery request, CancellationToken cancellationToken)
  {
    IEnumerable<AppDto> data = await _repo.Data
      .Select(row => new AppDto(
        row.Id,
        row.Index,
        row.Name,
        row.Description,
        row.CreatedAtUtc,
        row.Prefix
      ))
      .ToListAsync(cancellationToken);

    return data.Success();
  }
}
