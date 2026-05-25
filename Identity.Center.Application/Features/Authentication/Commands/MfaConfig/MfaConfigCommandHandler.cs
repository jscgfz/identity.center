using System.Net;
using System.Security.Claims;
using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Common;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Entities.Core.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OtpNet;
using QRCoder;

namespace Identity.Center.Application.Features.Authentication.Commands.MfaConfig;

internal sealed class MfaConfigCommandHandler(IServiceProvider provider) : ICommandHandler<MfaConfigCommand, BaseFileRender>
{
  private readonly IIdentityRepository<User> _userRepo = provider.GetRequiredService<IIdentityRepository<User>>();
  private readonly IIdentityUnitOfWork _unitOfWork = provider.GetRequiredService<IIdentityUnitOfWork>();
  private readonly IHttpContextAccessor _context = provider.GetRequiredService<IHttpContextAccessor>();

  public async Task<Result<BaseFileRender>> Handle(MfaConfigCommand request, CancellationToken cancellationToken)
  {
    if (
      _context.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier) is not Claim userClaim ||
      !Guid.TryParse(userClaim.Value, out Guid userId)
    )
      return Result.Result.Failure<BaseFileRender>(
        HttpStatusCode.Unauthorized,
        new BaseError("Invalid.Token", "Token invalido")
      );

    User? user = await _userRepo.Data
      .FirstOrDefaultAsync(row => row.Id == userId, cancellationToken);

    if (user == null)
      return Result.Result.Failure<BaseFileRender>(
        HttpStatusCode.Unauthorized,
        new BaseError("Invalid.Token", "Token invalido")
      );

    if (user.MfaSignature == null)
    {
      user.MfaSignature = KeyGeneration.GenerateRandomKey(32);
      _userRepo.Update(user);
      await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    string uri = new OtpUri(OtpType.Totp, Base32Encoding.ToString(user.MfaSignature), user.FullName, "identity").ToString();
    QRCodeData qrData = QRCodeGenerator.GenerateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
    PngByteQRCode pngQr = new(qrData);


    return new BaseFileRender(
      pngQr.GetGraphic(20),
      $"id-{user.Id}.png",
      "image/png"
    );
  }
}
