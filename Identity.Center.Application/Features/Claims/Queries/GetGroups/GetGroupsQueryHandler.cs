using Identity.Center.Application.Abstractions.Common;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Common;
using Identity.Center.Application.Features.Claims.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Entities.Core.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Features.Claims.Queries.GetGroups;

internal sealed class GetGroupsQueryHandler(IServiceProvider provider) : IQueryHandler<GetGroupsQuery, IPaginatedResult<MasterClaimPart>>
{
  private readonly IIdentityRepository<Group> _repo = provider.GetRequiredService<IIdentityRepository<Group>>();

  public async Task<Result<IPaginatedResult<MasterClaimPart>>> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
  {
    IPaginatedResult<MasterClaimPart> result = await PaginatedResult.ComputeAsync(
      _repo.Data
        .Where(row => string.IsNullOrWhiteSpace(request.Name) || row.Name.Contains(request.Name))
        .Where(row => string.IsNullOrWhiteSpace(request.Description) || (!string.IsNullOrWhiteSpace(row.Description) && row.Description.Contains(request.Description)))
        .OrderByDescending(row => row.CreatedAtUtc)
        .Select(row => new MasterClaimPart(
          row.Id,
          row.Name,
          row.Description,
          row.CreatedAtUtc
        )),
      request,
      cancellationToken
    );

    return result.AsResult();
  }
}
