using System.Net;
using System.Text.Json;
using Identity.Center.Application.Abstractions.Common;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Common;
using Identity.Center.Application.Common.Alfresco.Response;
using Identity.Center.Application.Extensions;
using Identity.Center.Application.Features.SelfHosting.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Entities.Core.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Features.SelfHosting.Queries.GetRequestRoles;

internal sealed class GetRequestRolesQueryHandler(IServiceProvider provider) : IQueryHandler<GetRequestRolesQuery, IPaginatedResult<RolePictureComparisonDto>>
{
  private readonly IIdentityRepository<ChangeControl> _change = provider.GetRequiredService<IIdentityRepository<ChangeControl>>();
  private readonly IHttpContextAccessor _context = provider.GetRequiredService<IHttpContextAccessor>();

  public async Task<Result<IPaginatedResult<RolePictureComparisonDto>>> Handle(GetRequestRolesQuery request, CancellationToken cancellationToken)
  {
    Func<ChangeControl, RolePictureComparisonDto> func = row =>
    {
      AlfrescoNodeEntryResponse alfresco = row.AuthorizationDocument.Deserialize<AlfrescoNodeEntryResponse>(JsonSerializerOptions.Web)!;
      RolePictureDto current = row.CurrentPicture.Deserialize<RolePictureDto>(JsonSerializerOptions.Web)!;
      RolePictureDto changes = row.RequestPicture.Deserialize<RolePictureDto>(JsonSerializerOptions.Web)!;
      return new RolePictureComparisonDto(
        row.Id,
        row.RoleId,
        new(
          alfresco.Entry.Id,
          alfresco.Entry.Name,
          alfresco.Entry.Content
        ),
        new(current.Name!, changes.Name),
        new(current.Description, changes.Description),
        new(current.DomainName!, changes.DomainName),
        new(current.ActiveDiretoryMandatory, changes.ActiveDiretoryMandatory),
        new(current.Root, changes.Root),
        new(current.Claims!, changes.Claims),
        new(row.CreatedBy, row.CreatedBy.ToString()),
        row.CreatedAtUtc,
        row.Status
      );
    };

    Result<Guid> appResult = await _context.RetrieveAppContext(cancellationToken);
    if (appResult.Failed)
      return Result.Result.Failure<IPaginatedResult<RolePictureComparisonDto>>(
        (HttpStatusCode)appResult.Code,
        appResult.Errors
      );

    IPaginatedResult<RolePictureComparisonDto> result = await PaginatedResult
      .ComputeAsync(
        _change.Data
          .Where(row => !request.Status.HasValue || row.Status == request.Status.Value)
          .Where(row => row.Role.AppId == appResult.Value)
          .OrderByDescending(row => row.CreatedAtUtc)
          .Select(row => func(row)),
        request,
        cancellationToken
      );

    return result.AsResult();
  }
}
