using Identity.Center.Application.Abstractions.Result;
using Identity.Center.Application.Common;

namespace Identity.Center.Application.Features.Authentication.Commands.MfaConfig;

public sealed record MfaConfigCommand() : ICommand<BaseFileRender>;
