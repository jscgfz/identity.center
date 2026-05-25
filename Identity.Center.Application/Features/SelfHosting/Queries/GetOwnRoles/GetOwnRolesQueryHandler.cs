using System.Net;
using System.Security.Claims;
using Identity.Center.Application.Abstractions.Common;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Common;
using Identity.Center.Application.Features.SelfHosting.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Features.SelfHosting.Queries.GetOwnRoles;

internal sealed class GetOwnRolesQueryHandler(IServiceProvider provider) : IQueryHandler<GetOwnRolesQuery, IPaginatedResult<OwnRoleDto>>
{
  private readonly IIdentityRepository<Role> _roleRepo = provider.GetRequiredService<IIdentityRepository<Role>>();
  private readonly IHttpContextAccessor _context = provider.GetRequiredService<IHttpContextAccessor>();

  public async Task<Result<IPaginatedResult<OwnRoleDto>>> Handle(GetOwnRolesQuery request, CancellationToken cancellationToken)
  {
    if (
      _context.HttpContext == null ||
      _context.HttpContext.User.FindFirstValue(IdentityClaimTypes.App) is not string appClaim ||
      !Guid.TryParse(appClaim, out Guid appId)
    )
      return Result.Result.Failure<IPaginatedResult<OwnRoleDto>>(
        HttpStatusCode.Unauthorized,
        new BaseError("Invalid.Token", "Token invalido")
      );

    IPaginatedResult<OwnRoleDto> result = await PaginatedResult
      .ComputeAsync(
        _roleRepo.Data
          .Where(row => row.AppId == appId)
          .Where(row => string.IsNullOrWhiteSpace(request.Name) || row.Name.Contains(request.Name))
          .Where(row => string.IsNullOrWhiteSpace(request.Description) || (!string.IsNullOrWhiteSpace(row.Description) && row.Description.Contains(request.Description)))
          .Where(row => string.IsNullOrWhiteSpace(request.DomainName) || (!string.IsNullOrWhiteSpace(row.DomainName) && row.DomainName.Contains(request.DomainName)))
          .Where(row => !request.Root.HasValue || row.Root == request.Root)
          .Where(row => string.IsNullOrWhiteSpace(request.Claim) || row.Claims.Any(c => (c.Claim.Group.Name + ":" + c.Claim.Action.Name).Contains(request.Claim)))
          .OrderByDescending(row => row.CreatedAtUtc)
          .Select(row => new OwnRoleDto(
            row.Id,
            row.Name,
            row.Description,
            row.DomainName,
            row.ActiveDirectoryMandatory,
            row.Root,
            row.CreatedAtUtc,
            row.Claims.Select(c => c.Claim.Group.Name + ":" + c.Claim.Action.Name)
          )),
        request,
        cancellationToken
      );

    return result.AsResult();
  }
}
