using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Apps.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Entities.Core.Builds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Features.Apps.Queries.GetApp;

internal sealed class GetAppQueryHandler(IServiceProvider provider) : IQueryHandler<GetAppQuery, AppDto>
{
  private readonly IIdentityRepository<App> _repo = provider.GetRequiredService<IIdentityRepository<App>>();

  public async Task<Result<AppDto>> Handle(GetAppQuery request, CancellationToken cancellationToken)
    => await _repo.Data
      .Where(row => row.Id == request.Id)
      .Select(row => new AppDto(
        row.Id,
        row.Index,
        row.Name,
        row.Description,
        row.CreatedAtUtc,
        row.Prefix
      ))
      .FirstAsync(cancellationToken);
}
