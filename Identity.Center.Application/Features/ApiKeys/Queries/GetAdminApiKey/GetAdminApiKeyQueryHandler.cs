using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.ApiKeys.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Entities.Core.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Features.ApiKeys.Queries.GetAdminApiKey;

internal sealed class GetAdminApiKeyQueryHandler(IServiceProvider provider) : IQueryHandler<GetAdminApiKeyQuery, ApiKeyDto>
{
  private readonly IIdentityRepository<ApiKey> _repo = provider.GetRequiredService<IIdentityRepository<ApiKey>>();

  public async Task<Result<ApiKeyDto>> Handle(GetAdminApiKeyQuery request, CancellationToken cancellationToken)
    => await _repo.Data
      .Where(row => row.Id == request.SubjectId)
      .Select(row => new ApiKeyDto(
        row.Id,
        row.Name,
        row.Description,
        new(
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
      ))
    .FirstAsync(cancellationToken);
}
