using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Identity.Center.Application.Abstractions.Clients;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Common.Alfresco.Response;
using Identity.Center.Application.Common.Options;
using Identity.Center.Application.Extensions;
using Identity.Center.Application.Features.SelfHosting.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Entities.Core.Authorization;
using Identity.Center.Domain.Entities.Core.Identity;
using Identity.Center.Domain.Entities.Core.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Identity.Center.Application.Features.SelfHosting.Commands.AddRequestRole;

internal sealed class AddRequestRoleCommandHandler(IServiceProvider provider) : ICommandHandler<AddRequestRoleCommand, ModifiedUserDto>
{
  private readonly IIdentityRepository<ChangeControl> _changes = provider.GetRequiredService<IIdentityRepository<ChangeControl>>();
  private readonly IIdentityRepository<Role> _role = provider.GetRequiredService<IIdentityRepository<Role>>();
  private readonly IIdentityUnitOfWork _unitOfWork = provider.GetRequiredService<IIdentityUnitOfWork>();
  private readonly IAlfrescoClient _alfresco = provider.GetRequiredService<IAlfrescoClient>();
  private readonly IOptionsMonitor<AlfrescoOptions> _options = provider.GetRequiredService<IOptionsMonitor<AlfrescoOptions>>();
  public async Task<Result<ModifiedUserDto>> Handle(AddRequestRoleCommand request, CancellationToken cancellationToken)
  {
    if (!_options.CurrentValue.NodeCollection.TryGetValue(nameof(Authorization), out string? nodeId))
      return Result.Result.Failure<ModifiedUserDto>(
        HttpStatusCode.Conflict,
        new BaseError("Alfresco.MissNode", "No se encontró configuración para el nodo")
      );

    HttpResponseMessage alfrescoUpload = await _alfresco.PostChildren(
      nodeId,
      request.Dto.AuthorizationFile.FileName,
      request.Dto.AuthorizationFile.AsStreamPart(),
      $"{nameof(Roles)}/{request.RoleId}",
      title: $"Solicitud de cambio {request.RoleId} - {DateTime.UtcNow:u}",
      description: request.Dto.Reason
    );

    if (!alfrescoUpload.IsSuccessStatusCode)
      return Result.Result.Failure<ModifiedUserDto>(
        alfrescoUpload.StatusCode,
        new JsonError("Alfresco.Error", await alfrescoUpload.Content.ReadFromJsonAsync<JsonElement>(cancellationToken))
      );

    AlfrescoNodeEntryResponse? entry = await alfrescoUpload.Content.ReadFromJsonAsync<AlfrescoNodeEntryResponse>(cancellationToken);

    if (entry == null)
      return Result.Result.Failure<ModifiedUserDto>(
        HttpStatusCode.Conflict,
        new BaseError("Alfresco.Error", "No se pudo deserializar la respuesta del servicio")
      );

    RolePictureDto currentPicture = await _role
      .Data
      .Include($"{nameof(Role.Claims)}.{nameof(RoleClaim.Claim)}.{nameof(ClaimValue.Group)}")
      .Include($"{nameof(Role.Claims)}.{nameof(RoleClaim.Claim)}.{nameof(ClaimValue.Action)}")
      .Where(row => row.Id == request.RoleId)
      .Select(row => new RolePictureDto(
        row.Name,
        row.Description,
        row.DomainName,
        row.ActiveDirectoryMandatory,
        row.Root,
        row.Claims.Select(c => c.Claim.Group.Name + ":" + c.Claim.Action.Name)
      ))
      .FirstAsync(cancellationToken);

    RolePictureDto requestPicture = new(
      request.Dto.Name,
      request.Dto.Description,
      request.Dto.DomainName,
      request.Dto.ActiveDiretoryMandatory,
      request.Dto.Root,
      request.Dto.Claims
    );

    ChangeControl control = new()
    {
      RoleId = request.RoleId,
      Reason = request.Dto.Reason,
      AuthorizationDocument = JsonSerializer.SerializeToElement(entry, JsonSerializerOptions.Web),
      CurrentPicture = JsonSerializer.SerializeToElement(currentPicture, JsonSerializerOptions.Web),
      RequestPicture = JsonSerializer.SerializeToElement(requestPicture, JsonSerializerOptions.Web)
    };

    await _changes.AddAsync(control, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new ModifiedUserDto(
      request.RoleId
    );
  }
}