using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.Apps.Dtos;

namespace Identity.Center.Application.Features.Apps.Commands.AddApp;

public sealed record AddAppCommand(
  string Prefix,
  string Name,
  string? DomainName,
  string? Description,
  bool? TwoFactorEnabled,
  TimeSpan? ExpirationTime,
  TimeSpan? RefreshTime
) : ICommand<CreatedAppDto>;
