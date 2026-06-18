using System.Net;
using System.Security.Claims;
using Identity.Center.Application.Abstractions.Common;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Common;
using Identity.Center.Application.Features.SelfHosting.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Authentication;
using Identity.Center.Domain.Entities.Core.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Features.SelfHosting.Queries.GetExternalUser;

internal sealed class GetExternalUserQueryHandler(IServiceProvider provider) : IQueryHandler<GetExternalUserQuery, IPaginatedResult<OwnUserDto>>
{
  private readonly IIdentityRepository<User> _userRepo = provider.GetRequiredService<IIdentityRepository<User>>();
  private readonly IHttpContextAccessor _context = provider.GetRequiredService<IHttpContextAccessor>();

  public async Task<Result<IPaginatedResult<OwnUserDto>>> Handle(GetExternalUserQuery request, CancellationToken cancellationToken)
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
          .Include(row => row.DomainCredentials)
          .ThenInclude(row => row.CredentialType)
          .Where(row => row.Roles.All(r => r.Role.AppId != appId))
          .Where(row => 
            string.IsNullOrWhiteSpace(request.Filter) ||
            row.DocumentType.Contains(request.Filter) ||
            row.DocumentNumber.Contains(request.Filter) ||
            row.FirstName.Contains(request.Filter) ||
            row.FirstLastName.Contains(request.Filter) ||
            (!string.IsNullOrWhiteSpace(row.SecondName) && row.SecondName.Contains(request.Filter)) ||
            (!string.IsNullOrWhiteSpace(row.SecondLastName) && row.SecondLastName.Contains(request.Filter)) ||
            row.ContactInfo.Any(ci => ci.Value.Contains(request.Filter)) ||
            row.DomainCredentials.Any(dc => dc.Username.Contains(request.Filter))
          )
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
            Enumerable.Empty<Guid>(),
            Enumerable.Empty<SingleCredential>(),
            row.DomainCredentials
          )),
        request,
        cancellationToken
      );

    return result.AsResult();
  }
}
