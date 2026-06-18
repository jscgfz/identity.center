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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Features.SelfHosting.Queries.GetOwnUsers;

internal sealed class GetOwnUsersQueryHandler(IServiceProvider provider) : IQueryHandler<GetOwnUsersQuery, IPaginatedResult<OwnUserDto>>
{
  private readonly IIdentityRepository<User> _userRepo = provider.GetRequiredService<IIdentityRepository<User>>();
  private readonly IHttpContextAccessor _context = provider.GetRequiredService<IHttpContextAccessor>();

  public async Task<Result<IPaginatedResult<OwnUserDto>>> Handle(GetOwnUsersQuery request, CancellationToken cancellationToken)
  {
    if (
      _context.HttpContext == null ||
      _context.HttpContext.User.FindFirstValue(IdentityClaimTypes.App) is not string appClaim ||
      !Guid.TryParse(appClaim, out Guid appId)
    )
      return Result.Result.Failure<IPaginatedResult<OwnUserDto>>(
        HttpStatusCode.Unauthorized,
        new BaseError("Invalid.Token", "Token invalido")
      );

    IPaginatedResult<OwnUserDto> result = await PaginatedResult
      .ComputeAsync(
        _userRepo.Data
          .Include(row => row.DomainCredentials.Where(dc => dc.CredentialType.Apps.Any(app => app.AppId == appId)))
          .ThenInclude(row => row.CredentialType)
          .Include(row => row.SingleCredentials.Where(sc => sc.AppId == appId))
          .Where(row => row.Roles.Any(r => r.Role.AppId == appId))
          .Where(row => string.IsNullOrWhiteSpace(request.DocumentType) || row.DocumentType.Contains(request.DocumentType))
          .Where(row => string.IsNullOrWhiteSpace(request.DocumentNumber) || row.DocumentNumber.Contains(request.DocumentNumber))
          .Where(row => string.IsNullOrWhiteSpace(request.FirstName) || row.FirstName.Contains(request.FirstName))
          .Where(row => string.IsNullOrWhiteSpace(request.FirstLastName) || row.FirstLastName.Contains(request.FirstLastName))
          .Where(row => string.IsNullOrWhiteSpace(request.ContactInfo) || row.ContactInfo.Any(ci => ci.Value.Contains(request.ContactInfo)))
          .Where(row => string.IsNullOrWhiteSpace(request.Role) || row.Roles.Any(r => r.Role.Name.Contains(request.Role)))
          .Where(row => string.IsNullOrWhiteSpace(request.SecondName) || (!string.IsNullOrWhiteSpace(row.SecondName) && row.SecondName.Contains(request.SecondName)))
          .Where(row => string.IsNullOrWhiteSpace(request.SecondLastName) || (!string.IsNullOrWhiteSpace(row.SecondLastName) && row.SecondLastName.Contains(request.SecondLastName)))
          .Where(row => string.IsNullOrWhiteSpace(request.Username) || (
            row.SingleCredentials.Any(sc => sc.Username.Contains(request.Username) && sc.AppId == appId) ||
            row.DomainCredentials.Any(dc => dc.Username.Contains(request.Username) && dc.CredentialType.Apps.Any(app => app.AppId == appId))
          ))
          .OrderByDescending(row => row.CreatedAtUtc)
          .Select(row => new OwnUserDto(
            row.Id,
            row.DocumentType,
            row.DocumentNumber,
            row.FirstName,
            row.SecondName,
            row.FirstLastName,
            row.SecondLastName,
            row.CreatedAtUtc,
            row.ContactInfo.Select(ci => new OwnContactInfoDto(
              ci.ContactType.ContactTypeKey,
              ci.Value,
              ci.Confirmed,
              ci.CreatedAtUtc
            )),
            row.Roles.Where(r => r.Role.AppId == appId)
              .Select(r => r.RoleId),
            row.SingleCredentials.Where(sc => sc.AppId == appId),
            row.DomainCredentials.Where(dc => dc.CredentialType.Apps.Any(app => app.AppId == appId))
          )),
        request,
        cancellationToken
      );

    return result.AsResult();
  }
}
