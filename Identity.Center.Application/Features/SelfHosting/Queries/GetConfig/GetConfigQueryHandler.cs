using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Builds;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Features.SelfHosting.Queries.GetConfig;

internal sealed class GetConfigQueryHandler(IServiceProvider provider) : IQueryHandler<GetConfigQuery, JsonElement>
{
  private readonly IIdentityRepository<AppConfigurationSection> _configRepo = provider.GetRequiredService<IIdentityRepository<AppConfigurationSection>>();
  private readonly IHttpContextAccessor _context = provider.GetRequiredService<IHttpContextAccessor>();

  public async Task<Result<JsonElement>> Handle(GetConfigQuery request, CancellationToken cancellationToken)
  {
    if (
      _context.HttpContext == null ||
      _context.HttpContext.User.FindFirstValue(IdentityClaimTypes.App) is not string appClaim ||
      !Guid.TryParse(appClaim, out Guid appId)
    )
      return Result.Result.Failure<JsonElement>(
        HttpStatusCode.Unauthorized,
        new BaseError("Invalid.Token", "Token invalido")
      );

    IEnumerable<AppConfigurationSection> configurationSections = await _configRepo.Data
      .Where(row => row.AppId == appId)
      .ToListAsync(cancellationToken);

    if(!configurationSections.Any())
      return Result.Result.Failure<JsonElement>(
        HttpStatusCode.NotFound,
        new BaseError("Config.NotFound", "No se encontro configuración para la aplicación")
      );

    IEnumerable<KeyValuePair<string, string?>> pairs = configurationSections
      .Select(x => KeyValuePair.Create(x.Key, x.Value))!;

    return JsonSerializer.SerializeToElement(pairs);
  }
}
