using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Apps.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Entities.Core.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Features.Apps.Queries.GetAppAuth;

internal sealed class GetAppAuthQueryHandler(IServiceProvider provider) : IQueryHandler<GetAppAuthQuery, AppAuthDto>
{
  private readonly IIdentityRepository<AppAuth> _repo = provider.GetRequiredService<IIdentityRepository<AppAuth>>();

  public async Task<Result<AppAuthDto>> Handle(GetAppAuthQuery request, CancellationToken cancellationToken)
    => await _repo.Data
      .Where(row => row.AppId == request.Id)
      .Select(row => new AppAuthDto(
        row.AppId,
        row.SignatureKey,
        row.TwoFactorEnabled,
        row.ExpirationTime,
        row.RefreshTime,
        row.CreatedAtUtc
      )).FirstAsync(cancellationToken);
}
