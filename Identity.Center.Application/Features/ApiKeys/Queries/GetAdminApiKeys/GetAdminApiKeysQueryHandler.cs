using Identity.Center.Application.Abstractions.Common;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Common;
using Identity.Center.Application.Features.ApiKeys.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Entities.Core.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Features.ApiKeys.Queries.GetAdminApiKeys;

internal sealed class GetAdminApiKeysQueryHandler(IServiceProvider provider) : IQueryHandler<GetAdminApiKeysQuery, IPaginatedResult<ApiKeyDto>>
{
  private readonly IIdentityRepository<ApiKey> _repo = provider.GetRequiredService<IIdentityRepository<ApiKey>>();

  public async Task<Result<IPaginatedResult<ApiKeyDto>>> Handle(GetAdminApiKeysQuery request, CancellationToken cancellationToken)
  {
    IPaginatedResult<ApiKeyDto> result = await PaginatedResult.ComputeAsync(
      _repo.Data
        .Where(row => string.IsNullOrWhiteSpace(request.Subject) || row.Id.ToString().Contains(request.Subject))
        .Where(row => string.IsNullOrWhiteSpace(request.Name) || row.Name.Contains(request.Name))
        .Where(row => string.IsNullOrWhiteSpace(request.Description) || (!string.IsNullOrWhiteSpace(row.Description) && row.Description.Contains(request.Description)))
        .Where(row => string.IsNullOrWhiteSpace(request.App) || row.App.Name.Contains(request.App) || (!string.IsNullOrWhiteSpace(row.App.DomainName) && row.App.DomainName.Contains(request.App)))
        .Where(row => string.IsNullOrWhiteSpace(request.Claims) || row.Claims.Any(c => (c.Claim.Group.Name + ":" + c.Claim.Action.Name).Contains(request.Claims)))
        .OrderByDescending(row => row.CreatedAtUtc)
        .Select(row => new ApiKeyDto(
          row.Id,
          row.Name,
          row.Description,
          new Apps.Dtos.AppDto(
            row.App.Id,
            row.App.Index,
            row.App.Name,
            row.App.DomainName,
            row.App.Description,
            row.App.CreatedAtUtc,
            row.App.Prefix
          ),
          row.CreatedAtUtc,
          row.Claims.Select(c => c.Claim.Group.Name + ":" + c.Claim.Action.Name)
        )),
      request,
      cancellationToken
    );

    return result.AsResult();
  }
}
