using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Common.Response;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Entities.Core.Builds;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Features.Types.Queries.GetCredentialTypes;

internal sealed class GetCredentialTypesQueryHandler(IServiceProvider provider) : IQueryHandler<GetCredentialTypesQuery, IEnumerable<MasterOption<int>>>
{
  private readonly IIdentityRepository<CredentialType> _repo = provider.GetRequiredService<IIdentityRepository<CredentialType>>();

  public async Task<Result<IEnumerable<MasterOption<int>>>> Handle(GetCredentialTypesQuery request, CancellationToken cancellationToken)
    => await _repo.Data
        .Select(row => new MasterOption<int>(row.Id, row.Name))
        .ToListAsync(cancellationToken);
}
