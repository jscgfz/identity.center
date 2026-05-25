using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Identity.Center.Application.Abstractions.Clients;
using Identity.Center.Application.Abstractions.Notifications;
using Identity.Center.Application.Common.Notifications.Masivian.Mail.Request;
using Identity.Center.Application.Common.Options;
using Identity.Center.Application.Result;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Identity.Center.Application.Common.Notifications.Masivian.Mail;

public sealed class MasivianEmailRequestBuilder() : INotificationBuilder
{
  private string? _templateName;
  private string? _body;
  private string? _subject;
  private readonly List<string> _recipient = [];
  private readonly Dictionary<string, string> _vars = [];

  public MasivianEmailRequestBuilder FromTamplate(string name)
  {
    _templateName = name;
    return this;
  }

  public MasivianEmailRequestBuilder WithSubject(string subject)
  {
    _subject = subject;
    return this;
  }

  public MasivianEmailRequestBuilder WithBody(string body)
  {
    _body = body;
    return this;
  }

  public MasivianEmailRequestBuilder WithVariable(string key, string value)
  {
    _vars.Add(key, value);
    return this;
  }

  public MasivianEmailRequestBuilder WithVariables(Dictionary<string, string> args)
  {
    foreach (KeyValuePair<string, string> pair in args)
      _vars.Add(pair.Key, pair.Value);
    return this;
  }

  public Task<Result<Unit>> EnQueueAsync(IServiceProvider provider, CancellationToken cancellationToken)
    => Task.FromResult(
      Result.Result.Failure<Unit>(
        HttpStatusCode.InternalServerError,
        new BaseError($"{nameof(EnQueueAsync)}.NotImplemented", "Método no implementado")
      )
    );

  public async Task<Result<Unit>> DispachAsync(IServiceProvider provider, CancellationToken cancellationToken)
  {
    IMasivianMailClient client = provider.GetRequiredService<IMasivianMailClient>();
    Result<MasivianEmailRequest> requestResult = Build(provider);
    if (requestResult.Failed)
      return requestResult.Map(_ => Unit.Value);
    HttpResponseMessage response = await client.DeliveryV1(requestResult.Value, cancellationToken);
    if (!response.IsSuccessStatusCode)
      return Result.Result.Failure<Unit>(
        response.StatusCode,
        new JsonError(
          "Masivian.Http",
          await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken)
        )
      );
    return Unit.Value;
  }

  private Result<MasivianEmailRequest> Build(IServiceProvider provider)
  {
    MasivianOptions options = provider
      .GetRequiredService<IOptionsMonitor<MasivianOptions>>()
      .CurrentValue;

    if (_body == null)
      if (_templateName == null)
        return Result.Result.Failure<MasivianEmailRequest>(
          HttpStatusCode.Conflict,
          new BaseError("Template.NotFound", "No se encontró informcaión de la plantilla")
        );
      else
      {
        string? body = provider.GetRequiredService<IConfiguration>()
          .GetSection($"{nameof(INotificationBuilder)}:{_templateName}:mail")
          .Get<string>();

        if (body == null)
          return Result.Result.Failure<MasivianEmailRequest>(
            HttpStatusCode.Conflict,
            new BaseError("Template.NotFound", "No se encontró informcaión de la plantilla")
          );

        _body = body;
      }

    foreach (KeyValuePair<string, string> pair in _vars)
      _body = _body.Replace($"[[{pair.Key}]]", pair.Value);

    return new MasivianEmailRequest(
      _subject!,
      options.Sender,
      _recipient.Select(r => new MasivianEmailRecipientRequest(r)),
      new MasivianEmailTemplateRequest(MasivianEmailParameterTypes.TextHtml, _body)
    );
  }
}
