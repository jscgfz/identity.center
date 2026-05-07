using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Center.Infrastructure.Configuration.Authorization;

internal sealed class IdentityAuthorizationHandler : IAuthorizationMiddlewareResultHandler
{
  private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

  public async Task HandleAsync(
    RequestDelegate next,
    HttpContext context,
    AuthorizationPolicy policy,
    PolicyAuthorizationResult authorizeResult
  )
  {
    if (authorizeResult.Forbidden)
    {
      string failureReason = authorizeResult.AuthorizationFailure?.FailureReasons.FirstOrDefault()?.Message
                          ?? "No cumples con los requisitos de seguridad para este recurso.";

      ProblemDetails problemDetails = new()
      {
        Status = StatusCodes.Status403Forbidden,
        Title = "Acceso Denegado (Forbidden)",
        Detail = failureReason,
        Instance = context.Request.Path,
        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
      };

      if (authorizeResult.AuthorizationFailure != null)
      {
        IEnumerable<IAuthorizationRequirement> requirementsNames = authorizeResult.AuthorizationFailure.FailedRequirements;
        problemDetails.Extensions["failedRequirements"] = requirementsNames.Select(req => JsonSerializer.SerializeToElement(req, req.GetType(), JsonSerializerOptions.Web));
      }

      context.Response.StatusCode = StatusCodes.Status403Forbidden;
      context.Response.ContentType = "application/problem+json";
      problemDetails.Extensions.TryAdd("method", context.Request.Method);
      problemDetails.Extensions.TryAdd("host", $"{context.Request.Scheme}://{context.Request.Host}");
      problemDetails.Extensions.TryAdd("requestId", context.TraceIdentifier);
      Activity? activity = context.Features.Get<IHttpActivityFeature>()?.Activity;
      problemDetails.Extensions.TryAdd("requestId", activity?.Id);

      await context.Response.WriteAsJsonAsync(problemDetails);
      return;
    }
    if (!authorizeResult.Succeeded)
    {
      ProblemDetails problemDetails = new()
      {
        Status = StatusCodes.Status401Unauthorized,
        Title = "Acceso No autorizado (Unauthorized)",
        Detail = "No cumples con la autorización para acceder al recurso",
        Instance = context.Request.Path,
        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
      };

      context.Response.StatusCode = StatusCodes.Status401Unauthorized;
      context.Response.ContentType = "application/problem+json";
      problemDetails.Extensions.TryAdd("method", context.Request.Method);
      problemDetails.Extensions.TryAdd("host", $"{context.Request.Scheme}://{context.Request.Host}");
      problemDetails.Extensions.TryAdd("requestId", context.TraceIdentifier);
      Activity? activity = context.Features.Get<IHttpActivityFeature>()?.Activity;
      problemDetails.Extensions.TryAdd("requestId", activity?.Id);

      await context.Response.WriteAsJsonAsync(problemDetails);

      return;
    }

    await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
  }
}
