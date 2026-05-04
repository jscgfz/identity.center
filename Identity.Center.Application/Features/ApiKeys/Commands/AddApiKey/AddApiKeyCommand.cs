using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Features.ApiKeys.Dtos;

namespace Identity.Center.Application.Features.ApiKeys.Commands.AddApiKey;

public sealed record AddApiKeyCommand(
  Guid AppId,
  bool Root,
  string Name,
  string? Description,
  IEnumerable<string> Claims
) : ICommand<CreatedApkiKeyDto>;
