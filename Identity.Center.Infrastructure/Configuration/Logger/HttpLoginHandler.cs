
using System.Text.Json;
using Identity.Center.Domain.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace Identity.Center.Infrastructure.Configuration.Logger;

internal sealed class HttpLoginHandler(IServiceProvider provider) : DelegatingHandler
{
  private readonly ILogger<HttpLoginHandler> _logger = provider.GetRequiredService<ILogger<HttpLoginHandler>>();

  protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
  {
    using (_logger.BeginScope("httpExecution"))
    {
      using MemoryStream stream = new();

      if (request.Content != null)
      {
        if(request.Content is MultipartFormDataContent cnt)
        {
          byte[] data = IdentityCommons.Encoding.GetBytes(
            JsonSerializer.Serialize(cnt)
          );

          stream.Write(data);
        }
        else
        {
          await request.Content.LoadIntoBufferAsync(cancellationToken);
          await request.Content.CopyToAsync(stream, cancellationToken);
        }
      }

      _logger.LogInformation(
        "Client executed request {method} {uri} - body {@body}",
        request.Method,
        request.RequestUri?.ToString(),
        JsonSerializer.Deserialize<JsonElement>(
          stream.Length > 0 ?
            IdentityCommons.Encoding.GetString(stream.ToArray()) :
            "no content"
        )
      );
      stream.Flush();
      stream.Position = 0;
      DateTimeOffset init = DateTimeOffset.UtcNow;
      HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

      using MemoryStream copyResponse = new();
      await response.Content.LoadIntoBufferAsync(cancellationToken);
      await response.Content.CopyToAsync(copyResponse, cancellationToken);
      _logger.LogInformation(
        "Client executed response code {statusCode} body {@body} elapsedTime {elapsedTime}",
        response.StatusCode,
        JsonSerializer.Deserialize<JsonElement>(
          stream.Length > 0 ?
            IdentityCommons.Encoding.GetString(copyResponse.ToArray()) :
            "no content"
        ),
        DateTimeOffset.UtcNow - init
      );

      return response;
    }
  }
}
