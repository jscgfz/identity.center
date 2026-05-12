using Identity.Center.Application.Abstractions.Common;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Common;
using Identity.Center.Application.Extensions;
using Identity.Center.Application.Features.Credentials.Dtos;
using Identity.Center.Application.Features.Roles.Dtos;
using Identity.Center.Application.Features.Users.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Entities.Core.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Features.Users.Queries.GetUsers;

internal sealed class GetUsersQueryHandler(IServiceProvider provider) : IQueryHandler<GetUsersQuery, IPaginatedResult<BasicUserInfoDto>>
{
  private readonly IHttpContextAccessor _http = provider.GetRequiredService<IHttpContextAccessor>();
  private readonly IIdentityRepository<User> _repo = provider.GetRequiredService<IIdentityRepository<User>>();

  public async Task<Result<IPaginatedResult<BasicUserInfoDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    => await _http.RetrieveAppContext(cancellationToken)
      .Bind(resp => Process(resp, request ,cancellationToken));

  private async Task<Result<IPaginatedResult<BasicUserInfoDto>>> Process(Guid appId, GetUsersQuery request, CancellationToken cancellationToken = default)
  {
    IPaginatedResult<BasicUserInfoDto> result = await PaginatedResult.ComputeAsync(
      _repo.Data
        .Where(row => string.IsNullOrWhiteSpace(request.DocumentType) || row.DocumentType.Contains(request.DocumentType))
        .Where(row => string.IsNullOrWhiteSpace(request.DocumentNumber) || row.DocumentNumber.Contains(request.DocumentNumber))
        .Where(row => string.IsNullOrWhiteSpace(request.FirstName) || row.FirstName.Contains(request.FirstName))
        .Where(row => string.IsNullOrWhiteSpace(request.FirstLastName) || row.FirstLastName.Contains(request.FirstLastName))
        .Where(row => string.IsNullOrWhiteSpace(request.SecondName) || (!string.IsNullOrWhiteSpace(row.SecondName) && row.SecondName.Contains(request.SecondName)))
        .Where(row => string.IsNullOrWhiteSpace(request.SecondLastName) || (!string.IsNullOrWhiteSpace(row.SecondLastName) && row.SecondLastName.Contains(request.SecondLastName)))
        .Where(row => string.IsNullOrWhiteSpace(request.ContactValue) || row.ContactInfo.Any(info => info.Value.Contains(request.ContactValue)))
        .Where(row => row.Roles.Any(r => r.Role.AppId == appId))
        .OrderByDescending(row => row.CreatedAtUtc)
        .Select(row => new BasicUserInfoDto(
          row.Id,
          row.DocumentType,
          row.DocumentNumber,
          row.FirstName,
          row.SecondName,
          row.FirstLastName,
          row.SecondLastName,
          row.CreatedAtUtc,
          row.ContactInfo.Select(info => new ContactInfoDto(
            info.Id,
            null,
            info.ContactType.ContactTypeKey,
            info.Value,
            info.Confirmed,
            info.CreatedAtUtc
          )),
          row.DomainCredentials.Where(
            dc => dc.CredentialType.Apps.Any(a => a.AppId == appId)
          ).Select(dc => new CreatedCredentialDto(
              dc.Id,
              null,
              dc.CredentialTypeId,
              dc.Username,
              dc.CreatedAtUtc
          )),
          row.SingleCredentials.Where(sc => sc.AppId == appId)
            .Select(sc => new CreatedCredentialDto(
              null,
              null,
              default,
              sc.Username,
              sc.CreatedAtUtc
            )),
          row.Roles.Where(r => r.Role.AppId == appId)
            .Select(r => new RoleDto(
              r.RoleId,
              null,
              r.Role.Name,
              r.Role.Description,
              r.Role.DomainName,
              r.Role.Root,
              r.Role.CreatedAtUtc
            ))
        )),
      request,
      cancellationToken
    );

    return result.AsResult();
  }
}
