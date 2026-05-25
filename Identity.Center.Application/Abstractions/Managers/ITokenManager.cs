using Identity.Center.Application.Features.Authentication.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Enums;
using MediatR;

namespace Identity.Center.Application.Abstractions.Managers;

public interface ITokenManager
{
  Task<Result<AuthenticationReponseDto>> FromUser(Guid userId, Guid appId, IEnumerable<string>? domainRoles = null, MfaStates? mfaOverride = null, CancellationToken cancellationToken = default);
  Task<Result<string>> RefreshToken(CancellationToken cancellationToken);
  Task<Result<Unit>> ValidateSession();
}
