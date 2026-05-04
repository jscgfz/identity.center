using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Common.Response;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Entities.Core.Security;
using Identity.Center.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Features.Types.Queries.GetContactTypes;

internal sealed class GetContactTypesQueryHandler(IServiceProvider provider) : IQueryHandler<GetContactTypesQuery, IEnumerable<MasterOption<ContactTypes>>>
{
  private readonly IIdentityRepository<ContactType> _repo = provider.GetRequiredService<IIdentityRepository<ContactType>>();

  public async Task<Result<IEnumerable<MasterOption<ContactTypes>>>> Handle(GetContactTypesQuery request, CancellationToken cancellationToken)
  {
    IEnumerable<MasterOption<ContactTypes>> options = await _repo.Data
      .Select(row => new MasterOption<ContactTypes>(
        row.ContactTypeKey,
        row.Name
      ))
      .ToListAsync(cancellationToken);

    return options.Success();
  }
}
