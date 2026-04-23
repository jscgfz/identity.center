using Identity.Center.Application.Abstractions.Repositories;
using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Apps.Dtos;
using Identity.Center.Application.Result;
using Identity.Center.Domain.Common;
using Identity.Center.Domain.Entities.Core.Builds;
using Identity.Center.Domain.Entities.Core.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Center.Application.Features.Apps.Commands.AddApp;

internal sealed class AddAppCommandHandler(IServiceProvider provider) : ICommandHandler<AddAppCommand, CreatedAppDto>
{
  private readonly IIdentityRepository<App> _appRepo = provider.GetRequiredService<IIdentityRepository<App>>();
  private readonly IIdentityRepository<AppAuth> _authRepo = provider.GetRequiredService<IIdentityRepository<AppAuth>>();
  private readonly IIdentityUnitOfWork _unitOfWork = provider.GetRequiredService<IIdentityUnitOfWork>();

  public async Task<Result<CreatedAppDto>> Handle(AddAppCommand request, CancellationToken cancellationToken)
  {
    App app = new()
    {
      Name = request.Name,
      Prefix = request.Prefix,
      DomainName = request.DomainName,
      Description = request.Description
    };

    await _appRepo.AddAsync(app, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    AppAuth appAuth = new()
    {
      AppId = app.Id,
      SignatureKey = IdentityCommons.NewHashKey
    };

    if(request.TwoFactorEnabled.HasValue)
      appAuth.TwoFactorEnabled = request.TwoFactorEnabled.Value;
    if(request.ExpirationTime.HasValue)
      appAuth.ExpirationTime = request.ExpirationTime.Value;
    if(request.RefreshTime.HasValue)
      appAuth.RefreshTime = request.RefreshTime.Value;

    await _authRepo.AddAsync(appAuth, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return new CreatedAppDto(app.Id);
  }
}
