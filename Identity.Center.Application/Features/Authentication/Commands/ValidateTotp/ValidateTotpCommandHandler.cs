using System.Net;
using System.Security.Claims;
using Identity.Center.Application.Abstractions.Managers;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Authentication.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Constants;
using Identity.Center.Domain.Entities.Core.Identity;
using Identity.Center.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OtpNet;

namespace Identity.Center.Application.Features.Authentication.Commands.ValidateTotp;

internal sealed class ValidateTotpCommandHandler(IServiceProvider provider) : ICommandHandler<ValidateTotpCommand, AuthenticationReponseDto>
{
  private readonly IIdentityRepository<User> _userRepo = provider.GetRequiredService<IIdentityRepository<User>>();
  private readonly IHttpContextAccessor _context = provider.GetRequiredService<IHttpContextAccessor>();
  private readonly ITokenManager _token = provider.GetRequiredService<ITokenManager>();

  public async Task<Result<AuthenticationReponseDto>> Handle(ValidateTotpCommand request, CancellationToken cancellationToken)
  {
    if (
      _context.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier) is not Claim userClaim ||
      !Guid.TryParse(userClaim.Value, out Guid userId) ||
      _context.HttpContext?.User.FindFirst(IdentityClaimTypes.App) is not Claim appClaim ||
      !Guid.TryParse(appClaim.Value, out Guid appId)
    )
      return Result.Result.Failure<AuthenticationReponseDto>(
        HttpStatusCode.Unauthorized,
        new BaseError("Invalid.Token", "Token invalido")
      );
    User? user = await _userRepo.Data
      .FirstOrDefaultAsync(row => row.Id == userId, cancellationToken);

    if (user == null)
      return Result.Result.Failure<AuthenticationReponseDto>(
        HttpStatusCode.Unauthorized,
        new BaseError("Invalid.Token", "Token invalido")
      );

    if (user.MfaSignature == null)
      return Result.Result.Failure<AuthenticationReponseDto>(
        HttpStatusCode.Unauthorized,
        new BaseError("Invalid.Totp", "Totp invalida")
      );

    Totp totp = new(user.MfaSignature);
    if (!totp.VerifyTotp(request.Totp, out _))
      return Result.Result.Failure<AuthenticationReponseDto>(
        HttpStatusCode.Unauthorized,
        new BaseError("Invalid.Totp", "Totp invalida")
      );

    return await _token.FromUser(userId, appId, mfaOverride: MfaStates.Passed, cancellationToken: cancellationToken);
  }
}
