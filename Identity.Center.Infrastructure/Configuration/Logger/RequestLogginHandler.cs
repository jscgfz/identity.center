using System.Text.Json;
using Identity.Center.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Identity.Center.Infrastructure.Configuration.Logger;

public sealed class RequestLogginHandler(RequestDelegate next)
{
  private readonly RequestDelegate _next = next;

  public async Task InvokeAsync(HttpContext context)
  {
    ILogger<RequestLogginHandler> _logger = context.RequestServices.GetRequiredService<ILogger<RequestLogginHandler>>();

    using (_logger.BeginScope($"request {context.TraceIdentifier}"))
    {
      DateTimeOffset init = DateTimeOffset.UtcNow;

      context.Request.EnableBuffering();
      string requestBody = "no content";

      using (MemoryStream requestMemo = new())
      {
        if(context.Request.ContentType?.Contains("form-data") ?? false)
        {
          byte[] data = IdentityCommons.Encoding.GetBytes(
            $"{context.Request.ContentType} - {context.Request.ContentLength}"
          );

          requestMemo.Write(data);
        }
        else
          await context.Request.Body.CopyToAsync(requestMemo);
        if (requestMemo.Length > 0)
        {
          requestBody = IdentityCommons.Encoding.GetString(requestMemo.ToArray());
        }
      }
      context.Request.Body.Position = 0;

      _logger.LogInformation(
          "request recieved {method} {path} - body {@body}",
          context.Request.Method,
          context.Request.Path,
          SafeDeserialize(requestBody)
      );

      Stream originalResponseBodyStream = context.Response.Body;
      using MemoryStream responseMemo = new();
      context.Response.Body = responseMemo;

      await _next(context);

      responseMemo.Position = 0;
      string responseBody = "no content";

      if (responseMemo.Length > 0)
      {
        responseBody = context.Response.ContentType?.Contains("json") ?? false ? IdentityCommons.Encoding.GetString(responseMemo.ToArray()) : context.Response.ContentType!;
      }

      _logger.LogInformation(
          "request finalized, elapsedTime {elapsedTime} statusCode {statusCode} body {@body}",
          DateTimeOffset.UtcNow - init,
          context.Response.StatusCode,
          SafeDeserialize(responseBody)
      );
      responseMemo.Position = 0;
      await responseMemo.CopyToAsync(originalResponseBodyStream);
      context.Response.Body = originalResponseBodyStream;
    }
  }

  private static object SafeDeserialize(string json)
  {
    if (string.IsNullOrWhiteSpace(json) || json == "no content")
      return "no content";

    try
    {
      return JsonSerializer.Deserialize<JsonElement>(json);
    }
    catch
    {
      return json;
    }
  }
}