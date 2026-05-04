using Identity.Center.Application.Abstractions.Common;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Common;
using Identity.Center.Application.Features.Apps.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Entities.Core.Builds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Features.Apps.Queries.GetApps;

internal sealed class GetAppsQueryHandler(IServiceProvider provider) : IQueryHandler<GetAppsQuery, IPaginatedResult<AppDto>>
{
  private readonly IIdentityRepository<App> _repo = provider.GetRequiredService<IIdentityRepository<App>>();
  public async Task<Result<IPaginatedResult<AppDto>>> Handle(GetAppsQuery request, CancellationToken cancellationToken)
  {
    IPaginatedResult<AppDto> data = await PaginatedResult.ComputeAsync(
      _repo.Data
      .OrderByDescending(row => row.CreatedAtUtc)
      .Where(row => string.IsNullOrEmpty(request.Prefix) || row.Prefix.Contains(request.Prefix))
      .Where(row => string.IsNullOrEmpty(request.Name) || row.Name.Contains(request.Name))
      .Where(row => string.IsNullOrEmpty(request.Description) || (!string.IsNullOrEmpty(row.Description) && row.Description.Contains(request.Description)))
      .Select(row => new AppDto(
        row.Id,
        row.Index,
        row.Name,
        row.DomainName,
        row.Description,
        row.CreatedAtUtc,
        row.Prefix
      )),
      request,
      cancellationToken
    );

    return data.AsResult();
  }
}
