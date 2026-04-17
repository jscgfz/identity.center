using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Builds;
using Identity.Center.Domain.Entities.Core.Security;
using Identity.Center.Persistence.Data.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Center.Infrastructure.Configuration.Authentication;

internal class JwtOptionsConfigurer(IServiceProvider provider) : IConfigureNamedOptions<JwtBearerOptions>
{
  private readonly IDbContextFactory<IdentityContext> _factory = provider.GetRequiredService<IDbContextFactory<IdentityContext>>();

  public void Configure(string? name, JwtBearerOptions options)
  {
    using IdentityContext context = _factory.CreateDbContext();
    options.TokenValidationParameters = new()
    {
      ValidateLifetime = true,
      ValidateIssuer = true,
      ClockSkew = TimeSpan.Zero,
      LogValidationExceptions = true,
      RequireAudience = true,
      RequireExpirationTime = true,
      RequireSignedTokens = true,
      ValidAlgorithms = [SecurityAlgorithms.HmacSha256Signature],
      IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) => IssuerSigningKeyResolver(token, securityToken, kid, validationParameters, context),
      ValidIssuers = context.Set<App>().Select(row => row.Prefix).AsEnumerable()
    };

  }

  public void Configure(JwtBearerOptions options)
    => Configure(JwtBearerDefaults.AuthenticationScheme, options);

  private IEnumerable<SecurityKey> IssuerSigningKeyResolver(string token, SecurityToken securityToken, string kid, TokenValidationParameters validationParameters, IdentityContext context)
  {
    JwtSecurityTokenHandler handler = new();
    JwtSecurityToken jwtSecurityToken = handler.ReadJwtToken(token);
    return jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == IdentityClaimTypes.App) is not Claim appIdClaim ||
      !Guid.TryParse(appIdClaim.Value, out Guid appId) ? [] :
      context.Set<AppAuth>().Where(row => row.AppId == appId).Select(row => new SymmetricSecurityKey(row.SignatureKey)).AsEnumerable();
  }
}
