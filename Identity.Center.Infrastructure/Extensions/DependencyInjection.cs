using Identity.Center.Infrastructure.Configuration.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Infrastructure.Extensions;

public static class DependencyInjection
{
  public static WebApplicationBuilder WithAuthentication(this WebApplicationBuilder builder)
  {
    builder
      .Services
      .ConfigureOptions<JwtOptionsConfigurer>();

    builder
      .Services
      .AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer();

    return builder;
  }
}
