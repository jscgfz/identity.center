using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Identity.Center.Application.Abstractions.Clients;
using Identity.Center.Application.Abstractions.Managers;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Common.Authentication.Models;
using Identity.Center.Application.Features.Authentication.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Common;
using Identity.Center.Domain.Common.Models.Cryptography;
using Identity.Center.Domain.Entities.Core.Authentication;
using Identity.Center.Domain.Entities.Core.Builds;
using Identity.Center.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Features.Authentication.Commands.Login;

internal sealed class LoginCommandHandler(IServiceProvider provider) : ICommandHandler<LoginCommand, AuthenticationReponseDto>
{
  private readonly IIdentityRepository<AppAllowedCredential> _allowedCredential = provider.GetRequiredService<IIdentityRepository<AppAllowedCredential>>();
  private readonly IIdentityRepository<DomainCredential> _domain = provider.GetRequiredService<IIdentityRepository<DomainCredential>>();
  private readonly IIdentityRepository<SingleCredential> _single = provider.GetRequiredService<IIdentityRepository<SingleCredential>>();
  private readonly ITokenManager _token = provider.GetRequiredService<ITokenManager>();
  private readonly IQdControlClient _client = provider.GetRequiredService<IQdControlClient>();

  public async Task<Result<AuthenticationReponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
  {
    IEnumerable<KeyValuePair<int, AuthenticationMethods>> allowedCredentials = await _allowedCredential.Data
      .AsNoTracking()
      .Where(row => row.AppId == request.AppId)
      .Select(row => KeyValuePair.Create(row.CredentialType.Id, row.CredentialType.AuthType))
      .ToListAsync(cancellationToken);

    if (!allowedCredentials.Any())
      return Result.Result.Failure<AuthenticationReponseDto>(
        HttpStatusCode.Conflict,
        new BaseError("AllowedCredential.NotConfigured", "No se han configurado las credenciales permitidas para esta aplicación")
      );

    IEnumerable<DomainCredential> domainCredentials = await _domain.Data
      .AsNoTracking()
      .Include(row => row.CredentialType)
      .Where(row => row.Username == request.Username && row.User.Roles.Any(r => r.Role.AppId == request.AppId))
      .ToListAsync(cancellationToken);

    SingleCredential? singleCredential = await _single.Data
      .AsNoTracking()
      .FirstOrDefaultAsync(row => row.Username == request.Username && row.AppId == request.AppId, cancellationToken);

    if (!domainCredentials.Any() && singleCredential == null)
      return Result.Result.Failure<AuthenticationReponseDto>(
        HttpStatusCode.Unauthorized,
        new BaseError("Credentials.Invalid", "Credenciales invalidas")

      );

    return domainCredentials.Any() ?
      await Domain(domainCredentials, request.Password, cancellationToken).Bind(resp => _token.FromUser(resp.Key, request.AppId, resp.Value, cancellationToken: cancellationToken)) :
      await Single(singleCredential!, request.Password).Bind(resp => _token.FromUser(resp, request.AppId, cancellationToken: cancellationToken));

  }

  private async Task<Result<KeyValuePair<Guid, IEnumerable<string>>>> Domain(
    IEnumerable<DomainCredential> domainCredentials,
    string password,
    CancellationToken cancellationToken
  )
  {
    IEnumerable<KeyValuePair<Guid, HttpResponseMessage>> responses = await Task.WhenAll(
      domainCredentials.Select(async row =>
      {
        QuamtumAuthAtomicValues atomicValues = row.CredentialType.Arguments.Deserialize<QuamtumAuthAtomicValues>(JsonSerializerOptions.Web)!;
        LdapAuthenticationRequest request = new(
          row.Username,
          password,
          atomicValues.DomainName,
          atomicValues.Key
        );

        return KeyValuePair.Create(
          row.UserId,
          await _client.Validate(request, cancellationToken)
        );
      })
    );

    if (responses.All(resp => !resp.Value.IsSuccessStatusCode))
      return Result.Result.Failure<KeyValuePair<Guid, IEnumerable<string>>>(
        responses.First().Value.StatusCode,
        new JsonError("QData.Error", await responses.First().Value.Content.ReadFromJsonAsync<JsonElement>(cancellationToken))
      );

    IEnumerable<KeyValuePair<Guid, LdapAuthenticationResponse>> contents = await Task.WhenAll(
      responses
        .Where(resp => resp.Value.IsSuccessStatusCode)
        .Select(async resp => KeyValuePair.Create(resp.Key, (await resp.Value.Content.ReadFromJsonAsync<LdapAuthenticationResponse>())!))
    );

    if (contents.All(c => c.Value.Message.Code != 0))
      return Result.Result.Failure<KeyValuePair<Guid, IEnumerable<string>>>(
        HttpStatusCode.Unauthorized,
        new BaseError("Credentials.Invalid", "Credenciales invalidas")
      );

    return KeyValuePair.Create(
      contents.First(c => c.Value.Message.Code == 0).Key,
      contents.First(c => c.Value.Message.Code == 0).Value.Roles.Select(r => r.Description)
    );
  }

  private static async Task<Result<Guid>> Single(
    SingleCredential credential,
    string password
  )
  {
    HashValidationResponse response = await IdentityCommons.ValidateHash(new(
      password,
      credential.Hash,
      credential.Salt
    ));

    return response.Success ?
      credential.UserId.Success() :
      Result.Result.Failure<Guid>(
        HttpStatusCode.Unauthorized,
        new BaseError("Credentials.Invalid", "Credenciales invalidas")
      );
  }
}
