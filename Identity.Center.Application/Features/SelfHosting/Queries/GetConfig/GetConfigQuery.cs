using System.Text.Json;
using Identity.Center.Application.Abstractions.Result;

namespace Identity.Center.Application.Features.SelfHosting.Queries.GetConfig;
public sealed record GetConfigQuery() : IQuery<JsonElement>;
